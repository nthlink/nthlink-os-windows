using nthLink.Header.Interface;
using nthLink.SDK.Extension;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace nthLink.Wpf.Model
{
    internal class UpdaterImp : IUpdater
    {
        const string CheckVersionURL = "https://www.downloadnth.com/versions.json";
        private readonly IContainerProvider containerProvider;
        private readonly ISystemReportLog? systemReportLog;
        private string downloadURL = string.Empty;
        public UpdaterImp(IContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
            this.systemReportLog = containerProvider.Resolve<ISystemReportLog>();
        }
        public async Task<string> Download()
        {
            if (!string.IsNullOrEmpty(this.downloadURL))
            {
                string tempFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()) + ".exe";

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        byte[] fileBytes = await client.GetByteArrayAsync(this.downloadURL);

                        await File.WriteAllBytesAsync(tempFileName, fileBytes);

                        return tempFileName;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return string.Empty;
        }

        public async Task<bool> NeedUpdate()
        {
            if (this.containerProvider.Resolve<IJsonConverter>() is IJsonConverter converter &&
                this.containerProvider.Resolve<IClientInfo>() is IClientInfo clientInfo)
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(CheckVersionURL);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // 解析JSON
                    Root? json = converter.Deserialize<Root>(responseBody);

                    if (json == null)
                    {
                        if (this.systemReportLog != null)
                        {
                            await this.systemReportLog.Log(Header.Enum.LogLevelEnum.Info,
                                $"Checking the last version failed, {responseBody}");
                        }
                    }
                    else
                    {
                        bool ok = false;
                        Version? newVersion = null;
                        string url = string.Empty;
                        //windows64 windows32
                        if (RuntimeInformation.ProcessArchitecture == Architecture.X86)
                        {
                            ok = Version.TryParse(json.windows32.version, out newVersion);
                            url = json.windows32.url;
                        }
                        else if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                        {
                            ok = Version.TryParse(json.windows64.version, out newVersion);
                            url = json.windows64.url;
                        }

                        if (ok &&
                            newVersion != null &&
                            newVersion > clientInfo.AppVersion)
                        {
                            this.downloadURL = url;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
#pragma warning disable CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。
        class Root
        {
            public Windows64 windows64 { get; set; }
            public Windows32 windows32 { get; set; }
        }

        class Windows32
        {
            public string version { get; set; }
            public string url { get; set; }
        }

        class Windows64
        {
            public string version { get; set; }
            public string url { get; set; }
        }
#pragma warning restore CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。
    }
}
