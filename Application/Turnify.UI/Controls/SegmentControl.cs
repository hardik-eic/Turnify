using Turnify.UI.Models;
namespace Turnify.UI.Controls;

public class SegmentControl : ContentView
{
	public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
		nameof(ItemsSource), typeof(IEnumerable<Segment>), typeof(SegmentControl), propertyChanged: OnItemsSourceChanged);

	public IEnumerable<Segment> ItemsSource
	{
		get => (IEnumerable<Segment>)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
		nameof(SelectedIndex), typeof(int), typeof(SegmentControl), -1, BindingMode.TwoWay);

	public int SelectedIndex
	{
		get => (int)GetValue(SelectedIndexProperty);
		set => SetValue(SelectedIndexProperty, value);
	}

	private HorizontalStackLayout _container;

	public SegmentControl()
	{
		_container = new HorizontalStackLayout
		{
			Spacing = 5,
			Padding = 10
		};

		Content = _container;
	}

	private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is SegmentControl control && newValue is IEnumerable<Segment> segments)
		{
			control.BuildSegments(segments);
		}
	}

	private void BuildSegments(IEnumerable<Segment> segments)
	{
		_container.Children.Clear();
		int index = 0;

		foreach (var segment in segments)
		{
			var segmentView = CreateSegmentView(segment, index);
			_container.Children.Add(segmentView);
			index++;
		}
	}

	private View CreateSegmentView(Segment segment, int index)
	{
		var image = new Image
		{
			Source = segment.ImageSource,
			WidthRequest = 40,
			HeightRequest = 40,
			HorizontalOptions = LayoutOptions.Center
		};

		var label = new Label
		{
			Text = segment.Text,
			FontSize = 14,
			FontAttributes = FontAttributes.Bold,
			HorizontalTextAlignment = TextAlignment.Center,
			HorizontalOptions = LayoutOptions.Center
		};

		var stack = new VerticalStackLayout
		{
			Children = { image, label },
			Padding = new Thickness(12, 8),
			Spacing = 2,
			BackgroundColor = segment.IsSelected ? Colors.LightGray : Colors.Transparent,
			VerticalOptions = LayoutOptions.Center,
			HorizontalOptions = LayoutOptions.Center
		};

		stack.GestureRecognizers.Add(new TapGestureRecognizer
		{
			Command = new Command(() =>
			{
				foreach (var child in _container.Children.OfType<VerticalStackLayout>())
				{
					child.BackgroundColor = Colors.Transparent;
				}
				stack.BackgroundColor = Colors.LightGray;
				SelectedIndex = index;
			})
		});

		return stack;
	}
}
