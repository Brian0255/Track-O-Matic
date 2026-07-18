namespace TrackOMatic
{
    static class Extensions
    {
        private static Random random = new();

        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            random = new(seed);
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static ItemVisibilityState ToItemVisibility(this System.Windows.Visibility visibility)
        {
            return visibility switch
            {
                System.Windows.Visibility.Visible => ItemVisibilityState.Visible,
                System.Windows.Visibility.Hidden => ItemVisibilityState.Hidden,
                System.Windows.Visibility.Collapsed => ItemVisibilityState.Collapsed,
                _ => throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null)
            };
        }

        public static System.Windows.Visibility ToWpfVisibility(this ItemVisibilityState visibility)
        {
            return visibility switch
            {
                ItemVisibilityState.Visible => System.Windows.Visibility.Visible,
                ItemVisibilityState.Hidden => System.Windows.Visibility.Hidden,
                ItemVisibilityState.Collapsed => System.Windows.Visibility.Collapsed,
                _ => throw new ArgumentOutOfRangeException(nameof(visibility), visibility, null)
            };
        }
    }
}
