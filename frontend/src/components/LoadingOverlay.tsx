import { Box, CircularProgress, Typography } from "@mui/material";

type LoadingOverlayProps = {
  loadingProgression: number;
  isVisible: boolean;
};

/**
 * ローディング進捗を表示するオーバーレイコンポーネント
 */
export const LoadingOverlay = ({ loadingProgression, isVisible }: LoadingOverlayProps) => {
  const progressPercent = Math.round(loadingProgression * 100);

  return (
    <Box
      sx={{
        position: "absolute",
        top: 0,
        left: 0,
        width: "100%",
        height: "100%",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        backgroundColor: "#1a1a2e",
        visibility: isVisible ? "visible" : "hidden",
        zIndex: 10,
      }}
    >
      <Box sx={{ position: "relative", display: "inline-flex" }}>
        <CircularProgress
          variant="determinate"
          value={progressPercent}
          size={120}
          thickness={4}
          sx={{ color: "#4fc3f7" }}
        />
        <Box
          sx={{
            top: 0,
            left: 0,
            bottom: 0,
            right: 0,
            position: "absolute",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <Typography variant="h5" component="div" sx={{ color: "#ffffff" }}>
            {progressPercent}%
          </Typography>
        </Box>
      </Box>
      <Typography variant="body1" sx={{ color: "#aaaaaa", mt: 3 }}>
        Loading...
      </Typography>
    </Box>
  );
};
