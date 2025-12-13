# React Frontend

このディレクトリには React アプリケーションが含まれています。

## 技術スタック

- **React 18** - UI ライブラリ
- **TypeScript** - 型安全性
- **Vite** - ビルドツール（高速な開発サーバー）
- **Biome** - リンター・フォーマッター（ESLint/Prettier の代替）

## 注意事項

- **Server Actions は使用しません** - このプロジェクトはクライアントサイドのみの React アプリケーションです
- Next.js は使用していません（Vite + React を使用）

## セットアップ

初回セットアップ時:

```bash
# frontendディレクトリで
pnpm install
```

## 開発サーバーの起動

```bash
pnpm run dev
```

## コードチェック

```bash
# チェック（リンター + フォーマット）を実行
pnpm run check

# チェック + 自動修正
pnpm run fix
```

## Unity ビルドの統合

1. Unity で WebGL ビルドを実行（`unity/Builds/WebGL/`に出力）
2. ルートディレクトリで以下を実行:
   ```bash
   pnpm run copy-unity
   ```
3. React アプリケーションで Unity を読み込み

## Unity コンポーネントの使用例

```tsx
import { Unity } from "react-unity-webgl";

function App() {
  return (
    <Unity
      loaderUrl="/unity/Build/UnityLoader.js"
      dataUrl="/unity/Build/Build.data"
      frameworkUrl="/unity/Build/Build.framework.js"
      codeUrl="/unity/Build/Build.wasm"
    />
  );
}
```
