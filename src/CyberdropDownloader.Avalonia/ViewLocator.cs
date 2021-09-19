using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CyberdropDownloader.Avalonia.ViewModels.Core;
using System;

namespace CyberdropDownloader.Avalonia
{
	public class ViewLocator : IDataTemplate
	{
		public bool SupportsRecycling => false;

		public IControl Build(object data)
		{
			string? name = data.GetType().FullName!.Replace("ViewModel", "View");
			Type? type = Type.GetType(name);

			if (type != null)
			{
				return (Control)Activator.CreateInstance(type)!;
			}
			else
			{
				return new TextBlock { Text = "Not Found: " + name };
			}
		}

		public bool Match(object data) => data is ViewModelBase;
	}
}
