import { SerializedError } from "@reduxjs/toolkit";
import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { useEffect, useState } from "react";

const useGetErrorMessageHook = (
  error: FetchBaseQueryError | SerializedError | undefined
): string | null => {
  const getErrorMessage = (
    error: FetchBaseQueryError | SerializedError | undefined
  ) => {
    if (!error) {
      return null; // Если ошибки нет, возвращаем null
    }

    if ("data" in error && error.data) {
      return error.data as string; // Возвращаем сообщение об ошибке, если оно присутствует
    }

    if ("error" in error && error.error) {
      return error.error; // Возвращаем сообщение об ошибке, если оно присутствует
    }

    if ("status" in error && error.status === 401) {
      return "Unauthorized"; // Обработка ошибки 401 (Unauthorized)
    }

    if ("status" in error && error.status === 403) {
      return "Forbid"; // Обработка ошибки 401 (Unauthorized)
    }

    if ("status" in error && error.status === 404) {
      return "Not Found"; // Обработка ошибки 404 (Not Found)
    }

    return null;
  };

  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    // Вызываем вашу функцию для получения сообщения об ошибке
    const newErrorMessage = getErrorMessage(error);
    // Устанавливаем новое сообщение об ошибке в состояние
    setErrorMessage(newErrorMessage);
  }, [error]); // Зависимость от error, чтобы useEffect запускался при его изменении

  return errorMessage;
};

export default useGetErrorMessageHook;
