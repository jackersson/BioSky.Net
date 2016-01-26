using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Reflection;
using System.Collections.ObjectModel;

namespace BioModule.DragDrop
{
  public class DragDrop
    {
     private Window          _topWindow;
     private Point           _initialMousePosition;
     private Canvas          _dragDropContainer; 
     private Boolean         _mouseCaptured;
     private UIElement       _dragDropPreviewControl;
     private Object          _dragDropPreviewControlDataContext;

     private static List<UIElement> dropTargetsList = new List<UIElement>();

     DropState CurrentDropState;

     private Point _delta;
     //private Point _lostFocus;
     public UIElement _dragSource;
     public UIElement _dropTarget;


     private static readonly Lazy<DragDrop> _Instance = new Lazy<DragDrop>(() => new DragDrop());

     private static DragDrop Instance
     {
         get { return _Instance.Value; }
     }

     private void DragSource_PreviewMouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
     {
         try
         {
             CurrentDropState = DropState.CannotDrop;
             var visual = e.OriginalSource as Visual;
             
             _topWindow = (Window)DragDrop.FindAncestor(typeof(Window), visual);
             
             _dragSource = (FindElementFromSource((DependencyObject)e.OriginalSource)) as UIElement;

             if (_dragSource == null)
             {
               return;
             }
             
             _initialMousePosition = e.GetPosition(_topWindow);
             
             _dragDropContainer = DragDrop.GetDragDropContainer(_dragSource as DependencyObject) as Canvas;

             if (_dragDropContainer == null)
             {                    
                 _dragDropContainer = (Canvas)DragDrop.FindAncestor(typeof(Canvas), visual);
             }

             FindVisualChildren(GetDropTargets(_dragSource as DependencyObject));

             if (_dragDropPreviewControlDataContext == null)
             { _dragDropPreviewControlDataContext = (_dragSource as FrameworkElement).DataContext; }
         }
         catch (Exception exc)
         {
             Console.WriteLine("Exception in DragDropHelper: " + exc.InnerException.ToString());
         }
     }   

     private void DragSource_PreviewMouseLeftButtonUp(Object sender, MouseButtonEventArgs e)
     {
         _dragDropPreviewControlDataContext = null;
         _mouseCaptured = false;

         if (_dragDropPreviewControl != null)
         { _dragDropPreviewControl.ReleaseMouseCapture(); }
     }
     
     private void DragSource_PreviewMouseMove(Object sender, MouseEventArgs e)
     {
       if (_mouseCaptured || _dragDropPreviewControlDataContext == null)
         return; //we're already capturing the mouse, or we don't have a data context for the preview control           
       
       if (DragDrop.IsMovementBigEnough(_initialMousePosition, e.GetPosition(_topWindow)) == false)
         return; //only drag when the user moved the mouse by a reasonable amount            
       
       _dragDropPreviewControl = GetDragDropPreviewControl(_dragSource as DependencyObject);

       if (_dragDropPreviewControl != null)
       {
         FrameworkElement dragDropPreviewControlAsFrameElement = (FrameworkElement)_dragDropPreviewControl;
         if (dragDropPreviewControlAsFrameElement != null)
           dragDropPreviewControlAsFrameElement.DataContext = _dragDropPreviewControlDataContext;
       }
       _dragDropPreviewControl.Opacity = 0.7;

       _dragDropContainer.Children.Add(_dragDropPreviewControl);
       _mouseCaptured = Mouse.Capture(_dragDropPreviewControl); //have the preview control recieve and be able to handle mouse events    

       //offset it just a bit so it looks like it's underneath the mouse
       Mouse.OverrideCursor = Cursors.Hand;

       _initialMousePosition = Mouse.GetPosition(_dragDropContainer);

       Canvas.SetLeft(_dragDropPreviewControl, _initialMousePosition.X - 20);
       Canvas.SetTop(_dragDropPreviewControl, _initialMousePosition.Y - 15);

       _dragDropContainer.PreviewMouseMove += DragDropContainer_PreviewMouseMove;
       _dragDropContainer.PreviewMouseUp += DragDropContainer_PreviewMouseUp;
     }
     
     private void DragDropContainer_PreviewMouseMove(Object sender, MouseEventArgs e)
     {
         var currentPoint = e.GetPosition(_dragDropContainer);

         //offset it just a bit so it looks like it's underneath the mouse
         Mouse.OverrideCursor = Cursors.Hand;
         currentPoint.X = currentPoint.X - 20;
         currentPoint.Y = currentPoint.Y - 15;

         _delta = new Point(_initialMousePosition.X - currentPoint.X, _initialMousePosition.Y - currentPoint.Y);
         var target = new Point(_initialMousePosition.X - _delta.X, _initialMousePosition.Y - _delta.Y);

         Canvas.SetLeft(_dragDropPreviewControl, target.X);
         Canvas.SetTop(_dragDropPreviewControl, target.Y);
/*
         if (target.X+20>=0 && target.Y+15>=0)
         {
           Canvas.SetLeft(_dragDropPreviewControl, target.X);
           Canvas.SetTop(_dragDropPreviewControl, target.Y);
           _lostFocus = new Point(_initialMousePosition.X - target.X - 20, _initialMousePosition.Y - target.Y - 15);              
         }*/
/*
         else
         {
           CurrentDropState = DropState.CannotDrop;
           return;
         }*/

         if (dropTargetsList == null)
           return;

         CurrentDropState = DropState.CannotDrop;

         foreach (UIElement elem in dropTargetsList)
         {
           var transform = elem.TransformToVisual(_dragDropContainer);
           var dropBoundingBox = transform.TransformBounds(new Rect(0, 0, elem.RenderSize.Width, elem.RenderSize.Height));

           if (e.GetPosition(_dragDropContainer).X > dropBoundingBox.Left &&
               e.GetPosition(_dragDropContainer).X < dropBoundingBox.Right &&
               e.GetPosition(_dragDropContainer).Y > dropBoundingBox.Top &&
               e.GetPosition(_dragDropContainer).Y < dropBoundingBox.Bottom)
           {
             _dropTarget = elem;
             CurrentDropState = DropState.CanDrop;
           }
         }   
     }      
     private static DoubleAnimation CreateDoubleAnimation(Double to)
     {
         var anim = new DoubleAnimation();
         anim.To = to;
         anim.Duration = TimeSpan.FromMilliseconds(250);
         anim.AccelerationRatio = 0.1;
         anim.DecelerationRatio = 0.9;

         return anim;
     }       
     private void DragDropContainer_PreviewMouseUp(Object sender, MouseEventArgs e)
     {
       switch (CurrentDropState)
         {
             case DropState.CanDrop:
                 try
                 {
                  /*   var scaleXAnim = CreateDoubleAnimation(0);
                     Storyboard.SetTargetProperty(scaleXAnim, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));

                     var scaleYAnim = CreateDoubleAnimation(0);
                     Storyboard.SetTargetProperty(scaleYAnim, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));

                     var opacityAnim = CreateDoubleAnimation(0);
                     Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("(UIElement.Opacity)"));

                     var canDropSb = new Storyboard() { FillBehavior = FillBehavior.Stop };
                     canDropSb.Children.Add(scaleXAnim);
                     canDropSb.Children.Add(scaleYAnim);
                     canDropSb.Children.Add(opacityAnim);
                     canDropSb.Completed += (s, args) => { FinalizePreviewControlMouseUp(); };
                     
                     FrameworkElement fe = (FrameworkElement)_dragDropPreviewControl;
                       if (fe != null)
                         canDropSb.Begin(fe);*/                
                   
                   IDragable dataContext = (IDragable)_dragDropPreviewControlDataContext;

                   if (dataContext == null)
                     return;

                   object dragableItem = dataContext.GetContext();
                   dataContext.ItemDragged(dragableItem);

                   FrameworkElement dropTargetFrameElement = (FrameworkElement)_dropTarget;
                   if (dropTargetFrameElement != null)
                   {
                     IDragable dataContextTarget = (IDragable)dropTargetFrameElement.DataContext;
                     if (dataContextTarget != null)
                       dataContextTarget.ItemDropped(dragableItem);
                   }                  
                 }
                 catch (Exception ex)
                 {
            Console.WriteLine(ex.Message);
          }
                 break;
             case DropState.CannotDrop:
                 try
                 {                     
/*
                   var translateXAnim = CreateDoubleAnimation(_lostFocus.X);
                     Storyboard.SetTargetProperty(translateXAnim, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"));

                     var translateYAnim = CreateDoubleAnimation(_lostFocus.Y);
                     Storyboard.SetTargetProperty(translateYAnim, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));

                     var opacityAnim = CreateDoubleAnimation(0);
                     opacityAnim.BeginTime = TimeSpan.FromMilliseconds(150);
                     Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("(UIElement.Opacity)"));

                     var cannotDropSb = new Storyboard() { FillBehavior = FillBehavior.Stop };
                     cannotDropSb.Children.Add(translateXAnim);
                     cannotDropSb.Children.Add(translateYAnim);
                     cannotDropSb.Children.Add(opacityAnim);
                     cannotDropSb.Completed += (s, args) => { FinalizePreviewControlMouseUp(); };

                     cannotDropSb.Begin((FrameworkElement)_dragDropPreviewControl);*/
                   
                 }
                 catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          }
                 break;
         }
       FinalizePreviewControlMouseUp();
         _dragDropPreviewControlDataContext = null;
         _mouseCaptured = false;
     }

     private void FinalizePreviewControlMouseUp()
     {
         _dragDropContainer.Children.Remove(_dragDropPreviewControl);
         _dragDropContainer.PreviewMouseMove -= DragDropContainer_PreviewMouseMove;
         _dragDropContainer.PreviewMouseUp -= DragDropContainer_PreviewMouseUp;

         if (_dragDropPreviewControl != null)
         {
             _dragDropPreviewControl.ReleaseMouseCapture();
         }
         _dragDropPreviewControl = null;
         Mouse.OverrideCursor = null;
     }


#region DependencyProperty

        public static readonly DependencyProperty IsDropTargetProperty = DependencyProperty.RegisterAttached(
             "IsDropTarget", typeof(Boolean), typeof(DragDrop), new PropertyMetadata(false));
        public static Boolean GetIsDropTarget(DependencyObject element)
        {
          return (Boolean)element.GetValue(IsDropTargetProperty);
        }
        public static void SetIsDropTarget(DependencyObject element, Boolean value)
        {
          element.SetValue(IsDropTargetProperty, value);
        }



        public static readonly DependencyProperty IsDragSourceProperty = DependencyProperty.RegisterAttached(
            "IsDragSource", typeof(Boolean), typeof(DragDrop), new PropertyMetadata(false, IsDragSourceChanged));
        public static Boolean GetIsDragSource(DependencyObject element)
        {          
            return (Boolean)element.GetValue(IsDragSourceProperty);          
        }
        public static void SetIsDragSource(DependencyObject element, Boolean value)
        {
            element.SetValue(IsDragSourceProperty, value);
        }       
        private static void IsDragSourceChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var dragSource = element as UIElement;
            
            if (dragSource == null)
            { return; }

            if (Object.Equals(e.NewValue, true))
            {
                dragSource.PreviewMouseLeftButtonDown += Instance.DragSource_PreviewMouseLeftButtonDown;
                dragSource.PreviewMouseLeftButtonUp += Instance.DragSource_PreviewMouseLeftButtonUp;
                dragSource.PreviewMouseMove += Instance.DragSource_PreviewMouseMove;              
            }
            else
            {
                dragSource.PreviewMouseLeftButtonDown -= Instance.DragSource_PreviewMouseLeftButtonDown;
                dragSource.PreviewMouseLeftButtonUp -= Instance.DragSource_PreviewMouseLeftButtonUp;
                dragSource.PreviewMouseMove -= Instance.DragSource_PreviewMouseMove;
            }
        }



        public static readonly DependencyProperty DragDropContainerProperty = DependencyProperty.RegisterAttached(
            "DragDropContainer", typeof(Panel), typeof(DragDrop), new PropertyMetadata(default(UIElement))); 
        public static Panel GetDragDropContainer(DependencyObject element)
        {
            return (Panel)element.GetValue(DragDropContainerProperty);
        }
        public static void SetDragDropContainer(DependencyObject element, Panel value)
        {
            element.SetValue(DragDropContainerProperty, value);
        }



        public static readonly DependencyProperty DragDropPreviewControlProperty = DependencyProperty.RegisterAttached(
            "DragDropPreviewControl", typeof(UIElement), typeof(DragDrop), new PropertyMetadata(default(UIElement)));
        public static UIElement GetDragDropPreviewControl(DependencyObject element)
        {
            return (UIElement)element.GetValue(DragDropPreviewControlProperty);
        }
        public static void SetDragDropPreviewControl(DependencyObject element, UIElement value)
        {
            element.SetValue(DragDropPreviewControlProperty, value);
        }

        

        public static readonly DependencyProperty DragDropPreviewControlDataContextProperty = DependencyProperty.RegisterAttached(
            "DragDropPreviewControlDataContext", typeof(Object), typeof(DragDrop), new PropertyMetadata(default(Object)));
        public static Object GetDragDropPreviewControlDataContext(DependencyObject element)
        {
            return (Object)element.GetValue(DragDropPreviewControlDataContextProperty);
        }
        public static void SetDragDropPreviewControlDataContext(DependencyObject element, Object value)
        {
            element.SetValue(DragDropPreviewControlDataContextProperty, value);
        }



        public static readonly DependencyProperty DropTargetsProperty = DependencyProperty.RegisterAttached(
            "DropTargets", typeof(UIElement), typeof(DragDrop), new PropertyMetadata(default(String)));
        public static UIElement GetDropTargets(DependencyObject element)
        {
          return (UIElement)element.GetValue(DropTargetsProperty);
        }
        public static void SetDropTargets(DependencyObject element, UIElement value)
        {
          element.SetValue(DropTargetsProperty, value);
        }
        #endregion


        #region Utilities
        public static FrameworkElement FindAncestor(Type ancestorType, Visual visual)
        {
            while (visual != null && !ancestorType.IsInstanceOfType(visual))
            {
                visual = (Visual)VisualTreeHelper.GetParent(visual);
            }
            return visual as FrameworkElement;
        }       
        public static Boolean IsMovementBigEnough(Point initialMousePosition, Point currentPosition)
        {
            return (Math.Abs(currentPosition.X - initialMousePosition.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPosition.Y - initialMousePosition.Y) >= SystemParameters.MinimumVerticalDragDistance);
        }

        public static void FindVisualChildren(DependencyObject depObj)
        {
          dropTargetsList.Clear();

          if (depObj == null)
            return;

          if (IsDropTargerContains(depObj))
            dropTargetsList.Add((UIElement)depObj);
          else
          {
            UIElement targetObject = (UIElement)depObj;
            if (targetObject == null)
              return;

            Queue<UIElement> elements = new Queue<UIElement>();
            elements.Enqueue(targetObject);
            while (elements.Count > 0)
            {
              UIElement ui = elements.Dequeue();
              for (int i = 0; i < VisualTreeHelper.GetChildrenCount(ui); i++)
              {
                DependencyObject child = VisualTreeHelper.GetChild(ui, i);
                if (child != null)
                {
                  UIElement currentElement = (UIElement)child;
                  if (currentElement != null)
                  {
                    if (IsDropTargerContains(child))
                      dropTargetsList.Add(currentElement);
                    else
                      elements.Enqueue(currentElement);
                  }
                }
              }
            }
          }
        }
        static object FindElementFromSource(DependencyObject source)
        {
          var dep = source;
          while ((dep != null))
          {
            dep = VisualTreeHelper.GetParent(dep);
            if (IsDragSourceContains(dep))
              return dep;

          }
          return null;
        }

        private static bool IsDropTargerContains(DependencyObject depObj)
        {
          FrameworkElement frameElement = (FrameworkElement)depObj;
          if (frameElement != null)
          {
            if (frameElement.DataContext is Caliburn.Micro.IScreen)
            {
              bool isDropTarget = GetIsDropTarget(depObj as DependencyObject);
              if (isDropTarget != false)
                return true;
            }
          }
          return false;
        }

        private static bool IsDragSourceContains(DependencyObject depObj)
        {
          FrameworkElement frameElement = (FrameworkElement)depObj;
          if (frameElement != null)
          {
            if (frameElement.DataContext is Caliburn.Micro.IScreen)
            {
              UIElement isDragSource = GetDropTargets(depObj as DependencyObject);
              if (isDragSource != null)
                return true;
            }
          }
          return false;
        } 

        #endregion
    }
}
