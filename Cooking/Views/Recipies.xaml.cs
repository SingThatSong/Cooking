﻿using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class Recepies : IRegionMemberLifetime
    {
        public Recepies()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
