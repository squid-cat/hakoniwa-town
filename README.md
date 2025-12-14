# hakoniwa-town

Unity WebGL + React プロジェクト

## セットアップ

### 必要な環境

- Node.js >= 20.19.0 (セキュリティパッチ適用済みを推奨)
  - セキュリティリリース: [2025 年 12 月 15 日のセキュリティリリース](https://nodejs.org/ja/blog/vulnerability/december-2025-security-releases) を参照
  - 最新の LTS バージョン（24.x）の使用を推奨
- pnpm >= 10.25.0
- Unity (WebGL ビルド対応)

### Node.js のバージョン管理 (nodenv-win)

このプロジェクトでは [nodenv-win](https://github.com/nodenv/nodenv-win) を使用してNode.jsのバージョンを管理しています。

**nodenv-win のインストール:**

1. nodenv-winをクローンします:
   ```bash
   git clone https://github.com/nodenv/nodenv-win.git $HOME/.nodenv
   ```

2. システムの環境変数に `%USERPROFILE%\.nodenv\bin` を追加します。

3. コマンドプロンプトまたはGit Bashを再起動します。

4. Node.js 20.19.0をインストールします:
   ```bash
   nodenv install 20.19.0
   nodenv local 20.19.0
   ```

プロジェクトディレクトリに `.node-version` ファイルがあるため、自動的にNode.js 20.19.0が使用されます。

### pnpm のインストール

```bash
npm install -g pnpm
```

### 依存関係のインストール

このプロジェクトは pnpm ワークスペースを使用しています。ルートディレクトリで一度インストールするだけで、すべてのワークスペース（frontend など）の依存関係がインストールされます。

```bash
# ルートディレクトリで（推奨）
pnpm install
```

これで、frontend を含むすべてのワークスペースの依存関係がインストールされます。

## 開発

### Unity ビルドの統合

1. Unity で WebGL ビルドを実行（`unity/Builds/WebGL/`に出力）
2. ルートディレクトリで以下を実行:
   ```bash
   pnpm run copy-unity
   ```

### React 開発サーバーの起動

```bash
# ルートディレクトリから
pnpm run dev

# または、frontendディレクトリで直接
cd frontend
pnpm run dev
```

### コードチェック

```bash
# チェック（リンター + フォーマット）を実行
pnpm run check

# チェック + 自動修正
pnpm run fix
```

### Git フック

このプロジェクトでは [husky](https://typicode.github.io/husky/) を使用して Git フックを管理しています。
`pnpm install` を実行すると、自動的に Git フックが設定されます。追加の設定は不要です。

- **pre-push**: プッシュ前に `pnpm check` を実行

チェックに失敗した場合、プッシュは中断されます。コードを修正してから再度プッシュしてください。


## ビルド

```bash
pnpm run build
```

## プロジェクト構成

詳細は [PROJECT_STRUCTURE.md](./PROJECT_STRUCTURE.md) を参照してください。
