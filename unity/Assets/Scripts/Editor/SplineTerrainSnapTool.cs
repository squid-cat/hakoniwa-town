using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public static class SplineTerrainSnapTool
{
    private const string MenuPathAll = "Tools/Spline/Subdivide And Snap Selected Spline To Terrain";
    private const float MaxKnotSpacingMeters = 8.0f;
    private const float HeightOffsetMeters = 0.05f;

    [MenuItem(MenuPathAll)]
    private static void SnapSelectedSplineToTerrain()
    {
        SplineContainer container = GetSelectedSplineContainer();
        if (container == null)
        {
            Debug.LogWarning("[SplineTerrainSnapTool] Select a GameObject with SplineContainer.");
            return;
        }

        Terrain[] terrains = Terrain.activeTerrains;
        if (terrains == null || terrains.Length == 0)
        {
            Debug.LogWarning("[SplineTerrainSnapTool] No active Terrain found.");
            return;
        }

        Undo.RecordObject(container, "Subdivide And Snap Spline To Terrain");
        Transform root = container.transform;
        int updatedCount = 0;
        int insertedCount = 0;

        for (int splineIndex = 0; splineIndex < container.Splines.Count; splineIndex++)
        {
            Spline spline = container.Splines[splineIndex];
            bool isClosed = spline.Closed;
            int sampleCount = GetSampleCount(spline, MaxKnotSpacingMeters);
            List<float3> snappedPositions = new List<float3>(sampleCount + 1);

            int loopCount = isClosed ? sampleCount : sampleCount + 1;
            for (int i = 0; i < loopCount; i++)
            {
                float t = sampleCount == 0 ? 0f : (float)i / sampleCount;
                float3 localPosition = spline.EvaluatePosition(t);
                Vector3 worldPosition = root.TransformPoint((Vector3)localPosition);

                if (!TryGetTerrainHeight(worldPosition, terrains, out float terrainY))
                {
                    terrainY = worldPosition.y;
                }

                worldPosition.y = terrainY + HeightOffsetMeters;
                snappedPositions.Add((float3)root.InverseTransformPoint(worldPosition));
            }

            int originalCount = spline.Count;
            List<BezierKnot> smoothKnots = BuildSmoothKnots(snappedPositions, isClosed);

            spline.Clear();
            for (int i = 0; i < smoothKnots.Count; i++)
            {
                spline.Add(smoothKnots[i], TangentMode.Broken);
            }

            spline.Closed = isClosed;
            insertedCount += Mathf.Max(0, snappedPositions.Count - originalCount);
            updatedCount += snappedPositions.Count;
        }

        EditorUtility.SetDirty(container);
        Debug.Log(
            $"[SplineTerrainSnapTool] Updated {updatedCount} knots (inserted {insertedCount}) on '{container.name}'. " +
            $"MaxSpacing={MaxKnotSpacingMeters}m Offset={HeightOffsetMeters}m");
    }

    [MenuItem(MenuPathAll, true)]
    private static bool ValidateSnapSelectedSplineToTerrain()
    {
        return GetSelectedSplineContainer() != null;
    }

    private static SplineContainer GetSelectedSplineContainer()
    {
        if (Selection.activeGameObject == null)
        {
            return null;
        }

        return Selection.activeGameObject.GetComponent<SplineContainer>();
    }

    private static bool TryGetTerrainHeight(Vector3 worldPosition, Terrain[] terrains, out float terrainY)
    {
        for (int i = 0; i < terrains.Length; i++)
        {
            Terrain terrain = terrains[i];
            if (terrain == null || terrain.terrainData == null)
            {
                continue;
            }

            Vector3 origin = terrain.transform.position;
            Vector3 size = terrain.terrainData.size;

            bool isInsideX = worldPosition.x >= origin.x && worldPosition.x <= origin.x + size.x;
            bool isInsideZ = worldPosition.z >= origin.z && worldPosition.z <= origin.z + size.z;

            if (!isInsideX || !isInsideZ)
            {
                continue;
            }

            terrainY = terrain.SampleHeight(worldPosition) + origin.y;
            return true;
        }

        terrainY = 0f;
        return false;
    }

    private static int GetSampleCount(Spline spline, float maxSpacingMeters)
    {
        if (spline == null || spline.Count < 2)
        {
            return 0;
        }

        float approxLength = 0f;
        Vector3 prev = (Vector3)spline[0].Position;
        for (int i = 1; i < spline.Count; i++)
        {
            Vector3 current = (Vector3)spline[i].Position;
            approxLength += Vector3.Distance(prev, current);
            prev = current;
        }

        if (spline.Closed)
        {
            approxLength += Vector3.Distance((Vector3)spline[spline.Count - 1].Position, (Vector3)spline[0].Position);
        }

        int sampleCount = Mathf.CeilToInt(approxLength / Mathf.Max(0.1f, maxSpacingMeters));
        return Mathf.Max(1, sampleCount);
    }

    private static List<BezierKnot> BuildSmoothKnots(List<float3> positions, bool isClosed)
    {
        List<BezierKnot> result = new List<BezierKnot>(positions.Count);
        if (positions == null || positions.Count == 0)
        {
            return result;
        }

        for (int i = 0; i < positions.Count; i++)
        {
            float3 current = positions[i];
            float3 previous;
            float3 next;

            if (isClosed)
            {
                previous = positions[(i - 1 + positions.Count) % positions.Count];
                next = positions[(i + 1) % positions.Count];
            }
            else
            {
                if (positions.Count == 1)
                {
                    previous = current;
                    next = current;
                }
                else if (i == 0)
                {
                    next = positions[1];
                    previous = current - (next - current);
                }
                else if (i == positions.Count - 1)
                {
                    previous = positions[positions.Count - 2];
                    next = current + (current - previous);
                }
                else
                {
                    previous = positions[i - 1];
                    next = positions[i + 1];
                }
            }

            BezierKnot knot = SplineUtility.GetAutoSmoothKnot(current, previous, next);
            result.Add(knot);
        }

        return result;
    }

}
