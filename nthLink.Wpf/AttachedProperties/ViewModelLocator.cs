using nthLink.Header.Interface;
using System;
using System.Collections.Generic;
using System.Windows;

namespace nthLink.Wpf.AttachedProperties
{
    public static class ViewModelLocator
    {
        private static readonly Dictionary<Type, Type> viewModelMapping = new Dictionary<Type, Type>();

        public static bool GetAutoWireViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoWireViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoAttach.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoWireViewModelProperty =
            DependencyProperty.RegisterAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(OnAutoWireViewModelPropertyValueChanged));

        private static void OnAutoWireViewModelPropertyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IContainerProvider containerProvider = App.ContainerProvider;

            if (e.NewValue is bool b)
            {
                if (b)
                {
                    if (d is FrameworkElement frameworkElement)
                    {
                        Type type = frameworkElement.GetType();

                        if (!viewModelMapping.ContainsKey(type) && !string.IsNullOrEmpty(type.Namespace))
                        {
                            string @namespace = type.Namespace;

                            if (@namespace.EndsWith(".Views"))
                            {
                                @namespace = @namespace.Replace(".Views", ".ViewModels");

                                string viewModelName = type.Name.EndsWith("View") ? "Model" : "ViewModel";

                                Type? viewModelType = type.Assembly.GetType($"{@namespace}.{type.Name}{viewModelName}");

                                if (viewModelType != null)
                                {
                                    viewModelMapping.Add(type, viewModelType);
                                }
                            }
                        }

                        if (viewModelMapping.ContainsKey(type))
                        {
                            frameworkElement.DataContext = containerProvider.Resolve(viewModelMapping[type]);
                        }
                    }
                }
            }
        }
    }
}