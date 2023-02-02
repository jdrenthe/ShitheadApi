using System.Threading.Tasks;
using ShitheadApi.Models;

namespace ShitheadApi.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<DataBundel<Email>> SendEmail(Email email);
    }
}
