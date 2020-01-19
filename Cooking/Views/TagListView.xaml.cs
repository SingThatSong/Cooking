﻿using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml.
    /// </summary>
    public partial class TagListView : IRegionMemberLifetime
    {
        public TagListView()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
