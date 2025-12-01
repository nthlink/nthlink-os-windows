using nthLink.Header.Interface;
using nthLink.SDK.Extension;
using nthLink.Wpf.Application.Services;
using nthLink.Wpf.Interface;
using nthLink.Wpf.MarkupExtension;
using nthLink.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace nthLink.Wpf.Infrastructure.DependencyInjection
{
    /// <summary>
    /// 服務註冊驗證器 - 用於開發階段驗證服務註冊的完整性
    /// </summary>
    public static class ServiceValidator
    {
        /// <summary>
        /// 驗證所有必要的服務是否已正確註冊
        /// </summary>
        public static ValidationResult ValidateServices(IContainerProvider containerProvider)
        {
            var result = new ValidationResult();
            var errors = new List<string>();
            var warnings = new List<string>();

            try
            {
                                // 驗證基礎設施層服務
                ValidateInfrastructureServices(containerProvider, errors);
                
                // 驗證應用層服務
                ValidateApplicationServices(containerProvider, errors);
                
                // 驗證表現層服務
                ValidatePresentationServices(containerProvider, errors);
                
                // 驗證遺留服務
                ValidateLegacyServices(containerProvider, errors);

                result.IsValid = errors.Count == 0;
                result.Errors = errors;
                result.Warnings = warnings;
            }
            catch (Exception ex)
            {
                errors.Add($"驗證過程中發生異常: {ex.Message}");
                result.IsValid = false;
                result.Errors = errors;
            }

            return result;
        }

        private static void ValidateInfrastructureServices(IContainerProvider containerProvider, List<string> errors)
        {
            ValidateService<IMainThreadSyncContext>(containerProvider, errors, "主執行緒同步上下文");
            ValidateService<ISystemReportLog>(containerProvider, errors, "系統報告日誌");
            ValidateService<IBypassSetProvider>(containerProvider, errors, "繞過設置提供者");
        }

        private static void ValidateApplicationServices(IContainerProvider containerProvider, List<string> errors)
        {
            ValidateService<IConnectionService>(containerProvider, errors, "連接服務");
            ValidateService<ILocalizationService>(containerProvider, errors, "本地化服務");
            ValidateService<ILanguageService>(containerProvider, errors, "語言服務（相容性）");
        }

        private static void ValidatePresentationServices(IContainerProvider containerProvider, List<string> errors)
        {
                        ValidateService<IDialogBox>(containerProvider, errors, "對話方塊服務");
            ValidateService<IToastWindow>(containerProvider, errors, "Toast視窗服務");
            
            // 驗證ViewModels
            ValidateService<NotifyItemViewModel>(containerProvider, errors, "通知項ViewModel");
            ValidateService<WebViewModel>(containerProvider, errors, "Web ViewModel");
            ValidateService<UpdateViewModel>(containerProvider, errors, "更新ViewModel");
            ValidateService<WebItemViewModel>(containerProvider, errors, "Web項ViewModel");
            ValidateService<NewsItemViewModel>(containerProvider, errors, "新聞項ViewModel");
        }

        private static void ValidateLegacyServices(IContainerProvider containerProvider, List<string> errors)
        {
            ValidateService<IWindowsRegister>(containerProvider, errors, "Windows註冊服務");
            ValidateService<TranslationSource>(containerProvider, errors, "翻譯源");
            ValidateService<Encoding>(containerProvider, errors, "編碼");
        }

        private static void ValidateService<T>(IContainerProvider containerProvider, List<string> errors, string serviceName)
        {
            try
            {
                var service = containerProvider.Resolve<T>();
                if (service == null)
                {
                    errors.Add($"{serviceName} ({typeof(T).Name}) 註冊了但解析為null");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"{serviceName} ({typeof(T).Name}) 解析失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 生成服務註冊報告
        /// </summary>
        public static string GenerateServiceReport(IContainerProvider containerProvider)
        {
            var result = ValidateServices(containerProvider);
            var report = new StringBuilder();

            report.AppendLine("=== 服務註冊驗證報告 ===");
            report.AppendLine($"驗證狀態: {(result.IsValid ? "✅ 通過" : "❌ 失敗")}");
            report.AppendLine($"驗證時間: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();

            if (result.Errors.Count > 0)
            {
                report.AppendLine("❌ 錯誤清單:");
                foreach (var error in result.Errors)
                {
                    report.AppendLine($"  • {error}");
                }
                report.AppendLine();
            }

            if (result.Warnings.Count > 0)
            {
                report.AppendLine("⚠️ 警告清單:");
                foreach (var warning in result.Warnings)
                {
                    report.AppendLine($"  • {warning}");
                }
                report.AppendLine();
            }

            if (result.IsValid)
            {
                report.AppendLine("✅ 所有必要的服務都已正確註冊！");
            }

            return report.ToString();
        }
    }

    /// <summary>
    /// 驗證結果
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}