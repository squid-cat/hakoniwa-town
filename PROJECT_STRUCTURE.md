# プロジェクト構成

## ディレクトリ構成

```
hakoniwa-town/
├── unity/                    # Unityプロジェクト（将来追加予定）
│   ├── Assets/
│   ├── Packages/
│   ├── ProjectSettings/
│   └── Builds/              # Unityのビルド出力（WebGL）
│       └── WebGL/           # WebGLビルドの出力先
│
├── frontend/                # Reactアプリケーション
│   ├── public/
│   │   └── unity/          # UnityのWebGLビルドを配置（自動コピー）
│   ├── src/
│   │   ├── pages/          # ページコンポーネント
│   │   │   └── Home.tsx
│   │   ├── App.tsx         # ルーティング設定
│   │   ├── main.tsx        # Reactエントリーポイント
│   │   ├── index.css       # グローバルスタイル
│   │   └── vite-env.d.ts   # Vite型定義
│   ├── biome.json          # Biome設定
│   ├── index.html          # HTMLエントリーポイント
│   ├── package.json
│   ├── tsconfig.json       # TypeScript設定
│   ├── tsconfig.node.json  # Node用TypeScript設定
│   └── vite.config.ts      # Vite設定
│
├── scripts/                 # ビルドスクリプトなど
│   └── copy-unity-build.js # UnityビルドをReactのpublicにコピー
│
├── .gitignore              # Git除外設定
├── .npmrc                  # npm/pnpm設定
├── .node-version           # Node.jsバージョン指定（nodenv用）
├── .nvmrc                  # Node.jsバージョン指定（nvm用）
├── pnpm-workspace.yaml     # pnpmワークスペース設定
├── package.json            # ルートのpackage.json（ワークスペース管理用）
├── README.md
└── PROJECT_STRUCTURE.md    # このファイル
```

## 構成の説明

### 1. `unity/` - Unity プロジェクト

- Unity プロジェクト全体を配置
- WebGL ビルドは `unity/Builds/WebGL/` に出力

### 2. `frontend/` - React アプリケーション

- **技術スタック**: React 18 + TypeScript + Vite
- **ルーティング**: React Router DOM
- **リンター・フォーマッター**: Biome
- `src/pages/` にページコンポーネントを配置
- `public/unity/` に Unity の WebGL ビルドを配置（自動コピー）
- Unity を React コンポーネントとして統合（将来実装予定）

### 3. `scripts/` - ビルドスクリプト

- Unity ビルドを React の public ディレクトリにコピーするスクリプト
- CI/CD パイプラインで使用

## ワークフロー

1. **Unity で開発・ビルド**

   - Unity エディタでプロジェクトを開発
   - WebGL プラットフォームでビルド → `unity/Builds/WebGL/` に出力

2. **React に統合**

   - ビルドスクリプトで `unity/Builds/WebGL/` を `frontend/public/unity/` にコピー
   - React コンポーネントで Unity を読み込み

3. **開発・デプロイ**
   - React 開発サーバーで動作確認
   - 本番ビルド時に Unity ビルドも含めてデプロイ

## パッケージマネージャー

このプロジェクトでは **pnpm** を使用しています。

### pnpm のインストール

```bash
npm install -g pnpm
```

### 使用方法

```bash
# 依存関係のインストール（ワークスペース全体）
pnpm install

# 開発サーバーの起動
pnpm run dev

# ビルド
pnpm run build

# コードチェック
pnpm run check

# コードチェック + 自動修正
pnpm run fix

# Unityビルドのコピー
pnpm run copy-unity
```

### ワークスペース構成

このプロジェクトは pnpm ワークスペースを使用しています：

- ルート: プロジェクト全体の管理
- `frontend`: React アプリケーション

`pnpm-workspace.yaml` でワークスペースを定義しています。
