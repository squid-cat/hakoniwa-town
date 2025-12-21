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

## 環境変数の設定

`frontend`ディレクトリに`.env`ファイルを作成してください：

```bash
# frontend/.env
VITE_SOCKET_URL=http://localhost:3001
VITE_BASE_URL=http://localhost:3000
```

### 環境変数の説明

- `VITE_SOCKET_URL`: WebSocketサーバーのURL（デフォルト: `http://localhost:3001`）
- `VITE_BASE_URL`: アプリケーションのベースURL（デフォルト: `window.location.origin`）

### 別端末からアクセスする場合

別端末（スマートフォンなど）からアクセスする場合は、ローカルネットワークのIPアドレスを指定してください：

```bash
# frontend/.env
VITE_SOCKET_URL=http://192.168.1.100:3001
VITE_BASE_URL=http://192.168.1.100:3000
```

`192.168.1.100`は、開発マシンのローカルIPアドレスに置き換えてください。IPアドレスは以下のコマンドで確認できます：

- Windows: `ipconfig`
- macOS/Linux: `ifconfig` または `ip addr`

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
