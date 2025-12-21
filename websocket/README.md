# WebSocket Server

このディレクトリには Socket.IO を使用した WebSocket サーバーが含まれています。

## 技術スタック

- **Node.js** - ランタイム
- **TypeScript** - 型安全性
- **Express** - HTTP サーバー
- **Socket.IO** - WebSocket 通信
- **tsx** - TypeScript 実行環境（開発用）

## セットアップ

初回セットアップ時:

```bash
# ルートディレクトリで（推奨）
pnpm install

# または、websocketディレクトリで直接
cd websocket
pnpm install
```

## 環境変数の設定

`websocket`ディレクトリに`.env`ファイルを作成してください：

```bash
# websocket/.env
PORT=3001
HOST=localhost
CORS_ORIGIN=http://localhost:3000
```

環境変数が設定されていない場合、デフォルト値が使用されます：

- `PORT`: 3001
- `HOST`: localhost（デフォルトはローカルのみアクセス可能）
- `CORS_ORIGIN`: http://localhost:3000

### 特定の IP アドレスでリッスンする場合

特定の IP アドレス（例: 192.168.11.5）でリッスンする場合は、`HOST`環境変数に IP アドレスを指定してください：

```bash
# websocket/.env
PORT=3001
HOST=192.168.11.5
CORS_ORIGIN=http://192.168.11.5:3000
```

## 開発サーバーの起動

```bash
# ルートディレクトリから
pnpm run dev:websocket

# または、websocketディレクトリで直接
cd websocket
pnpm run dev
```

## 実装されている機能

- **ルーム参加/退出**: `join-room` / `leave-room` イベント
- **メッセージ送信**: `message` イベント（ルーム内ブロードキャスト）
- **ユーザー参加/退出通知**: `user-joined` / `user-left` イベント

## サーバー側のイベント

### クライアント → サーバー

- `join-room` (roomId: string) - ルームに参加
- `leave-room` (roomId: string) - ルームから退出
- `message` (data: any) - メッセージを送信（ルーム内ブロードキャスト）

### サーバー → クライアント

- `user-joined` - 他のユーザーがルームに参加したとき
- `user-left` - 他のユーザーがルームから退出したとき
- `message` - ルーム内のメッセージを受信
