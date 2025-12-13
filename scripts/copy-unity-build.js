#!/usr/bin/env node

/**
 * UnityのWebGLビルドをReactのpublicディレクトリにコピーするスクリプト
 * 
 * 使用方法:
 *   node scripts/copy-unity-build.js
 */

const fs = require('fs');
const path = require('path');

const UNITY_BUILD_DIR = path.join(__dirname, '..', 'unity', 'Builds', 'WebGL');
const REACT_PUBLIC_UNITY_DIR = path.join(__dirname, '..', 'frontend', 'public', 'unity');

function copyRecursiveSync(src, dest) {
  const exists = fs.existsSync(src);
  const stats = exists && fs.statSync(src);
  const isDirectory = exists && stats.isDirectory();

  if (isDirectory) {
    if (!fs.existsSync(dest)) {
      fs.mkdirSync(dest, { recursive: true });
    }
    fs.readdirSync(src).forEach((childItemName) => {
      copyRecursiveSync(
        path.join(src, childItemName),
        path.join(dest, childItemName)
      );
    });
  } else {
    if (!fs.existsSync(path.dirname(dest))) {
      fs.mkdirSync(path.dirname(dest), { recursive: true });
    }
    fs.copyFileSync(src, dest);
  }
}

function main() {
  console.log('UnityビルドをReactのpublicディレクトリにコピー中...');

  if (!fs.existsSync(UNITY_BUILD_DIR)) {
    console.error(`エラー: Unityビルドディレクトリが見つかりません: ${UNITY_BUILD_DIR}`);
    console.error('UnityでWebGLビルドを実行してください。');
    process.exit(1);
  }

  // 既存のunityディレクトリを削除
  if (fs.existsSync(REACT_PUBLIC_UNITY_DIR)) {
    console.log('既存のunityディレクトリを削除中...');
    fs.rmSync(REACT_PUBLIC_UNITY_DIR, { recursive: true, force: true });
  }

  // コピー実行
  console.log(`${UNITY_BUILD_DIR} -> ${REACT_PUBLIC_UNITY_DIR}`);
  copyRecursiveSync(UNITY_BUILD_DIR, REACT_PUBLIC_UNITY_DIR);

  console.log('✅ コピーが完了しました！');
}

main();

