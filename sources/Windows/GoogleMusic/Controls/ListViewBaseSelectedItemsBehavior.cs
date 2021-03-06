﻿// --------------------------------------------------------------------------------------------------------------------
// OutcoldSolutions (http://outcoldsolutions.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.GoogleMusic.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading.Tasks;

    using OutcoldSolutions.Diagnostics;

    using Windows.UI.Core;
    using Windows.UI.Interactivity;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    public class ListViewBaseSelectedItemsBehavior : Behavior<ListViewBase>
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems",
            typeof(object),
            typeof(ListViewBaseSelectedItemsBehavior),
            new PropertyMetadata(null, (o, args) => 
            {
                ((ListViewBaseSelectedItemsBehavior)o).OnSelectedItemsChanged(args);
            }));

        public static readonly DependencyProperty ForceToShowProperty = DependencyProperty.Register(
            "ForceToShow", typeof(bool), typeof(ListViewBaseSelectedItemsBehavior), new PropertyMetadata(false));

        private static readonly Lazy<ILogger> Logger = new Lazy<ILogger>(() => ApplicationBase.Container.Resolve<ILogManager>().CreateLogger(typeof(ListViewBaseSelectedItemsBehavior).Name));

        private bool freezed = false;

        public object SelectedItems
        {
            get { return (object)this.GetValue(SelectedItemsProperty); }
            set { this.SetValue(SelectedItemsProperty, value); }
        }

        public bool ForceToShow
        {
            get { return (bool)this.GetValue(ForceToShowProperty); }
            set { this.SetValue(ForceToShowProperty, value); }
        }
        
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.SelectionChanged += this.OnSelectionChanged;
            this.Synchronize();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.SelectionChanged -= this.OnSelectionChanged;
        }

        private void OnSelectedItemsChanged(DependencyPropertyChangedEventArgs args)
        {
            var oldNotifyCollectionChanged = args.OldValue as INotifyCollectionChanged;
            if (oldNotifyCollectionChanged != null)
            {
                oldNotifyCollectionChanged.CollectionChanged -= this.OnCollectionChanged;
            }

            var newNotifyCollectionChanged = args.NewValue as INotifyCollectionChanged;
            if (newNotifyCollectionChanged != null)
            {
                newNotifyCollectionChanged.CollectionChanged += this.OnCollectionChanged;
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.freezed)
            {
                return;
            }

            var collection = this.SelectedItems as IList;
            if (collection != null)
            {
                if (e.AddedItems == null && e.RemovedItems == null)
                {
                    collection.Clear();
                }
                else
                {
                    this.freezed = true;

                    if (e.RemovedItems != null)
                    {
                        var removedItems = e.RemovedItems.Where(collection.Contains).ToList();

                        foreach (object item in removedItems)
                        {
                            collection.Remove(item);
                        }
                    }

                    if (e.AddedItems != null)
                    {
                        var addedItems = e.AddedItems.Where(x => !collection.Contains(x)).ToList();

                        foreach (object item in addedItems)
                        {
                            collection.Add(item);
                        }
                    }

                    this.freezed = false;
                }
            }
        }

        private async void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.freezed)
            {
                return;
            }

            if (this.AssociatedObject != null)
            {
                if (e.NewItems == null && e.OldItems == null)
                {
                    if (this.AssociatedObject.SelectedItems != null)
                    {
                        this.AssociatedObject.SelectedItems.Clear();
                    }
                }
                else
                {
                    this.freezed = true;

                    if (e.OldItems != null)
                    {
                        if (this.AssociatedObject.SelectedItems != null)
                        {
                            foreach (object item in e.OldItems)
                            {
                                if (this.AssociatedObject.SelectedItems.Contains(item))
                                {
                                    this.AssociatedObject.SelectedItems.Remove(item);
                                }
                            }
                        }
                    }

                    if (e.NewItems != null)
                    {
                        if (this.AssociatedObject.SelectedItems != null)
                        {
                            foreach (object item in e.NewItems)
                            {
                                if (!this.AssociatedObject.SelectedItems.Contains(item))
                                {
                                    this.AssociatedObject.SelectedItems.Add(item);
                                }
                            }
                        }
                    }

                    this.freezed = false;
                }

                if (this.ForceToShow && e.NewItems != null && this.AssociatedObject.SelectedItems !=null && this.AssociatedObject.SelectedItems.Count == 1)
                {
                    await Task.Yield();

                    await this.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Low, () =>
                                {
                                    try
                                    {
                                        if (this.AssociatedObject != null && e.NewItems.Count > 0)
                                        {
                                            this.AssociatedObject.ScrollIntoView(e.NewItems[0]);
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        Logger.Value.Debug(exception, "OnCollectionChanged");
                                    }
                                });
                }
            }
        }

        private async void Synchronize()
        {
            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.SelectedItems.Clear();

                var collection = this.SelectedItems as IList;
                if (collection != null)
                {
                    foreach (var selectedItem in collection)
                    {
                        this.AssociatedObject.SelectedItems.Add(selectedItem);
                    }
                }

                if (this.ForceToShow && this.AssociatedObject.SelectedItems.Count == 1)
                {
                    await Task.Yield();

                    await this.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Low, () =>
                                {
                                    try
                                    {
                                        if (this.AssociatedObject != null && this.AssociatedObject.SelectedItems.Count > 0)
                                        {
                                            this.AssociatedObject.ScrollIntoView(this.AssociatedObject.SelectedItems[0]);
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        Logger.Value.Debug(exception, "Synchronize");
                                    }
                                });
                }
            }
        }
    }
}
