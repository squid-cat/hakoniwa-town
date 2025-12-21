/**
 * UUID v4を生成します。
 * crypto.randomUUID()が利用できない環境でも動作します。
 *
 * @returns UUID v4形式の文字列
 */
export const generateUUID = (): string => {
  // crypto.randomUUID()が利用可能な場合はそれを使用
  if (typeof crypto !== "undefined" && crypto.randomUUID) {
    return crypto.randomUUID();
  }

  // フォールバック: UUID v4を手動生成
  return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, (c) => {
    const r = (Math.random() * 16) | 0;
    const v = c === "x" ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
};
