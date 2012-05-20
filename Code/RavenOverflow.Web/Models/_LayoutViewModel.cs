using RavenOverflow.Web.Models.Authentication;

namespace RavenOverflow.Web.Models
{
    // ReSharper disable InconsistentNaming
    
    public class _LayoutViewModel
    {
        public _LayoutViewModel(ICustomIdentity customIdentity)
        {
            CustomIdentity = customIdentity;
        }

        #region Implementation of _ILayoutViewModel

        public ICustomIdentity CustomIdentity { get; private set; }
        public string Header { get; set; }

        #endregion
    }

    // ReSharper restore InconsistentNaming
}