import { Link } from "react-router-dom";

export const NotFound = () => {
  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "100vh",
        gap: "20px",
      }}
    >
      <h1 style={{ fontSize: "48px", margin: 0 }}>404</h1>
      <h2 style={{ fontSize: "24px", margin: 0 }}>ページが見つかりません</h2>
      <p>指定されたURLのページは存在しません。</p>
      <Link
        to="/"
        style={{
          padding: "10px 20px",
          backgroundColor: "#1976d2",
          color: "white",
          textDecoration: "none",
          borderRadius: "4px",
        }}
      >
        ホームに戻る
      </Link>
    </div>
  );
};
