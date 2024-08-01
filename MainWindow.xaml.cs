using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using RPISSVR_Managements.view.Insert_Infos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RPISSVR_Managements
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow m_AppWindow;
        public MainWindow()
        {

            this.InitializeComponent();
            
            m_AppWindow = this.AppWindow;
            //m_AppWindow.Changed += AppWindow_Changed;
            Activated += MainWindow_Activated;
            AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
            AppTitleBar.Loaded += AppTitleBar_Loaded;

            ExtendsContentIntoTitleBar = true;
            if (ExtendsContentIntoTitleBar == true)
            {
                m_AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            }
            TitleBarTextBlock.Text = AppInfo.Current.DisplayInfo.DisplayName;
        }

        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (ExtendsContentIntoTitleBar == true)
            {
                // Set the initial interactive regions.
                SetRegionsForCustomTitleBar();
            }
        }

        private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ExtendsContentIntoTitleBar == true)
            {
                // Update interactive regions if the size of the window changes.
                SetRegionsForCustomTitleBar();
            }
        }

        private void SetRegionsForCustomTitleBar()
        {
            // Specify the interactive regions of the title bar.

            double scaleAdjustment = AppTitleBar.XamlRoot.RasterizationScale;

            RightPaddingColumn.Width = new GridLength(m_AppWindow.TitleBar.RightInset / scaleAdjustment);
            LeftPaddingColumn.Width = new GridLength(m_AppWindow.TitleBar.LeftInset / scaleAdjustment);

            // Get the rectangle around the AutoSuggestBox control.
            GeneralTransform transform = TitleBarSearchBox.TransformToVisual(null);
            Rect bounds = transform.TransformBounds(new Rect(0, 0,
                                                             TitleBarSearchBox.ActualWidth,
                                                             TitleBarSearchBox.ActualHeight));
            Windows.Graphics.RectInt32 SearchBoxRect = GetRect(bounds, scaleAdjustment);

            // Get the rectangle around the PersonPicture control.
            transform = PersonPic.TransformToVisual(null);
            bounds = transform.TransformBounds(new Rect(0, 0,
                                                        PersonPic.ActualWidth,
                                                        PersonPic.ActualHeight));
            Windows.Graphics.RectInt32 PersonPicRect = GetRect(bounds, scaleAdjustment);

            var rectArray = new Windows.Graphics.RectInt32[] { SearchBoxRect, PersonPicRect };

            Microsoft.UI.Input.InputNonClientPointerSource nonClientInputSrc =
                InputNonClientPointerSource.GetForWindowId(this.AppWindow.Id);
            nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
        }

        private Windows.Graphics.RectInt32 GetRect(Rect bounds, double scale)
        {
            return new Windows.Graphics.RectInt32(
                _X: (int)Math.Round(bounds.X * scale),
                _Y: (int)Math.Round(bounds.Y * scale),
                _Width: (int)Math.Round(bounds.Width * scale),
                _Height: (int)Math.Round(bounds.Height * scale)
            );
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                TitleBarTextBlock.Foreground =
                    (SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"];
            }
            else
            {
                TitleBarTextBlock.Foreground =
                    (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
            }
        }

        //private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
        //{
        //    if (args.DidPresenterChange)
        //    {
        //        switch (sender.Presenter.Kind)
        //        {
        //            case AppWindowPresenterKind.CompactOverlay:
        //                // Compact overlay - hide custom title bar
        //                // and use the default system title bar instead.
        //                AppTitleBar.Visibility = Visibility.Collapsed;
        //                sender.TitleBar.ResetToDefault();
        //                break;

        //            case AppWindowPresenterKind.FullScreen:
        //                // Full screen - hide the custom title bar
        //                // and the default system title bar.
        //                AppTitleBar.Visibility = Visibility.Collapsed;
        //                sender.TitleBar.ExtendsContentIntoTitleBar = true;
        //                break;

        //            case AppWindowPresenterKind.Overlapped:
        //                // Normal - hide the system title bar
        //                // and use the custom title bar instead.
        //                AppTitleBar.Visibility = Visibility.Visible;
        //                sender.TitleBar.ExtendsContentIntoTitleBar = true;
        //                break;

        //            default:
        //                // Use the default system title bar.
        //                sender.TitleBar.ResetToDefault();
        //                break;
        //        }
        //    }
        //}

        //private void SwitchPresenter(object sender, RoutedEventArgs e)
        //{
        //    if (AppWindow != null)
        //    {
        //        AppWindowPresenterKind newPresenterKind;
        //        switch ((sender as Button).Name)
        //        {
        //            case "CompactoverlaytBtn":
        //                newPresenterKind = AppWindowPresenterKind.CompactOverlay;
        //                break;

        //            case "FullscreenBtn":
        //                newPresenterKind = AppWindowPresenterKind.FullScreen;
        //                break;

        //            case "OverlappedBtn":
        //                newPresenterKind = AppWindowPresenterKind.Overlapped;
        //                break;

        //            default:
        //                newPresenterKind = AppWindowPresenterKind.Default;
        //                break;
        //        }

        //        // If the same presenter button was pressed as the
        //        // mode we're in, toggle the window back to Default.
        //        if (newPresenterKind == AppWindow.Presenter.Kind)
        //        {
        //            AppWindow.SetPresenter(AppWindowPresenterKind.Default);
        //        }
        //        else
        //        {
        //            // Else request a presenter of the selected kind
        //            // to be created and applied to the window.
        //            AppWindow.SetPresenter(newPresenterKind);
        //        }
        //    }
        //}

        private void MainNV_Load(object sender, RoutedEventArgs e)
        {

        }

        private void Main_NV_Items_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (S1.IsSelected)
            {
                var tabViewItem = new TabViewItem();              
                tabViewItem.Header = "បញ្ចូលទិន្នន័យនិស្សិត";
                var frame = new Frame();
                frame.Navigate(typeof(P_InsertStudents_Info));
                tabViewItem.Content = frame;

                TabView.TabItems.Add(tabViewItem);
                TabView.SelectedIndex = 1;

                

                

                


            }

        }
        public void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Item);
            

        }

        private void TabView_TabItemsChanged(TabView sender, IVectorChangedEventArgs args)
        {

        }
    }
}
