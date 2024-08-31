using System.Threading.Tasks;

namespace nthLink.Wpf.Interface
{
    public interface IDialogBox
    {
        Task<bool> ShowDialog(string title, string message, string trueOption, string falseOption);
        Task ShowDialog(string title, string message, string trueOption);
    }
}
