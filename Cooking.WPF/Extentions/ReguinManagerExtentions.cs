using Prism.Regions;

namespace Cooking.WPF.Services
{
    /// <summary>
    /// Extentions for <see cref="IRegionManager"/>.
    /// </summary>
    public static class ReguinManagerExtentions
    {
        /// <summary>
        /// Navigate for main region.
        /// </summary>
        /// <param name="regionManager">Manager to execute.</param>
        /// <param name="view">View to navigate to.</param>
        /// <param name="parameters">Parameters to pass to ViewModel.</param>
        public static void NavigateMain(this IRegionManager regionManager, string view, params (string, object)[] parameters) => regionManager.RequestNavigate(Consts.MainContentRegion, view, parameters);

        /// <summary>
        /// Navigate for main region.
        /// </summary>
        /// <param name="regionManager">Manager to execute.</param>
        /// <param name="region">Region for navigation.</param>
        /// <param name="view">View to navigate to.</param>
        /// <param name="parameters">Parameters to pass to ViewModel.</param>
        public static void RequestNavigate(this IRegionManager regionManager, string region, string view, params (string, object)[] parameters)
        {
            var navigationParams = new NavigationParameters();

            foreach ((string key, object val) in parameters)
            {
                navigationParams.Add(key, val);
            }

            regionManager.RequestNavigate(region, view, navigationParams);
        }
    }
}
