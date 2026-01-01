using UnityEngine;

public class DisplayDetailTextManager : MonoBehaviour
{
    [SerializeField] private GameObject detailTextPanel;

    public void SwitchDisplayDetailText()
    {
        detailTextPanel.SetActive(!detailTextPanel.activeSelf);
    }
}
