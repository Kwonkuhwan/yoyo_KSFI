using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LJS
{
    public abstract class Util
    {
        public delegate void DelegateMessage(object message, LogType type);
        public static event DelegateMessage OnMessage;
        public delegate void DelegateLogs(string logs);
        public static event DelegateLogs OnLogs;
        private static readonly StringBuilder DebugLogList = new StringBuilder();
        private static DateTime _checkTime;
        private const string ObjectNamePattern = @"\(([^)]*)\)";

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Log(object message)
        {
            Debug.unityLogger.Log(LogType.Log, message);
            OnMessage?.Invoke(message, LogType.Log);
            SetStringBuild(message, LogType.Log);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Log(object message, Object context)
        {
            Debug.unityLogger.Log(LogType.Log, message, context);
            OnMessage?.Invoke(message, LogType.Log);
            SetStringBuild(message, LogType.Log);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogError(object message)
        {
            Debug.unityLogger.Log(LogType.Error, message);
            OnMessage?.Invoke(message, LogType.Error);
            SetStringBuild(message, LogType.Error);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogError(object message, Object context)
        {
            Debug.unityLogger.Log(LogType.Error, message, context);
            OnMessage?.Invoke(message, LogType.Error);
            SetStringBuild(message, LogType.Error);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogWarning(object message)
        {
            Debug.unityLogger.Log(LogType.Warning, message);
            OnMessage?.Invoke(message, LogType.Warning);
            SetStringBuild(message, LogType.Warning);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogWarning(object message, Object context)
        {
            Debug.unityLogger.Log(LogType.Warning, message, context);
            OnMessage?.Invoke(message, LogType.Warning);
            SetStringBuild(message, LogType.Warning);
        }

        private static void SetStringBuild(object message, LogType type)
        {
            if (_checkTime.Second != DateTime.Now.Second ||
                _checkTime.Minute != DateTime.Now.Minute ||
                _checkTime.Hour != DateTime.Now.Hour ||
                _checkTime.Year != DateTime.Now.Year)
            {
                _checkTime = DateTime.Now;
                DebugLogList.AppendLine($"\n{_checkTime}");
                DebugLogList.AppendLine($"----------------------------------");
            }

            switch (type)
            {
                case LogType.Error:
                    DebugLogList.AppendLine($"<color=red>{message}</color>");
                    break;
                case LogType.Assert:
                    break;
                case LogType.Warning:
                    DebugLogList.AppendLine($"<color=yellow>{message}</color>");
                    break;
                case LogType.Log:
                    DebugLogList.AppendLine($"{message}");

                    break;
                case LogType.Exception:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            OnLogs?.Invoke(DebugLogList.ToString());
        }

        public static string GetLogMessage()
        {
            return DebugLogList.ToString();
        }

        public static void ClearLog()
        {
            DebugLogList.Clear();
        }

        /// <summary>
        /// pattern = @"\(([^)]*)\)": 정규식 패턴에서 \(와 \)는 각각 여는 괄호와 닫는 괄호를 의미하고, [^)]*는 닫는 괄호가 나오기 전까지의 모든 문자열을 의미합니다.
        /// Match.Groups[1].Value: 첫 번째 그룹은 괄호 안의 문자열을 나타냅니다.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetObjectName(string input)
        {
            var match = Regex.Match(input, ObjectNamePattern);
            return match.Success ? match.Groups[1].Value : input;
        }
        
        /// <summary>
        /// 정규식을 이용하여 문자열에서 모든 공백을 제거하는 함수
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveWhitespaceUsingRegex(string input)
        {
            return Regex.Replace(input, @"\s+", "");  // 모든 공백 문자 (스페이스, 탭, 개행) 제거
        }
    }
}