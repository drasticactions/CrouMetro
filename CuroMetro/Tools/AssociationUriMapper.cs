using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Croumetro.Tools
{
    internal class AssociationUriMapper : UriMapperBase
    {
        private string _tempUri;

        public override Uri MapUri(Uri uri)
        {
            _tempUri = HttpUtility.UrlDecode(uri.ToString());
            if (!_tempUri.Contains("?code=")) return uri;
            int authCodeIndex = _tempUri.IndexOf("code=", StringComparison.Ordinal) + 5;
            string authCode = _tempUri.Substring(authCodeIndex);
            return new Uri("/LaunchPage.xaml?code=" + authCode, UriKind.Relative);
        }
    }
}
