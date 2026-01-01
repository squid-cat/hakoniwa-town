import { useUnityContext } from "react-unity-webgl";

/**
 * Unity WebGLコンテキストを提供するカスタムフック
 *
 * @returns UnityコンテキストとsendMessage関数
 */
export const useUnity = () => {
  const props = useUnityContext({
    loaderUrl: "Build/public.loader.js",
    dataUrl: "Build/public.data",
    frameworkUrl: "Build/public.framework.js",
    codeUrl: "Build/public.wasm",
  });

  return props;
};
