using System;
using System.Collections.Generic;

public enum PopupStates
{
	NotOpen = 0,
	Open,
	Closed
}

[Flags]
public enum PopupResults
{
	None = 0,
	OK = 1,
	Cancel = 2,
	Yes = 4,
	No = 8,
	Close = 16
}

public static class PopupEnumExtentions
{
	public static bool IsNone(this PopupResults code)
	{
		return code == PopupResults.None;
	}

	public static bool IsClosed(this PopupStates code)
	{
		return code == PopupStates.Closed;
	}
}

public class PopupState
{
	public delegate void Handler(PopupState state);

	public PopupStates State = PopupStates.NotOpen;
	public PopupResults Result
	{
		get { return result; }
		set
		{
			result = value;
			State  = PopupStates.Closed;
			CallHandler(value);
			if (result != PopupResults.Close)
				CallHandler(PopupResults.Close);
			handlers.Clear();
		}
	}

	public object ResultParam { get; set; }

	public void SetResult(PopupResults result, object param)
	{
		ResultParam = param;
		Result = result;
	}

	private void CallHandler(PopupResults result)
	{
		Handler handler = GetHandler(result);
		if (handler != null)
			handler(this);
	}

	public Handler OnOK
	{
		get { return GetHandler(PopupResults.OK); }
		set { SetHandler(PopupResults.OK, value); }
	}

	public Handler OnCancel
	{
		get { return GetHandler(PopupResults.Cancel); }
		set { SetHandler(PopupResults.Cancel, value); }
	}

	public Handler OnYes
	{
		get { return GetHandler(PopupResults.Yes); }
		set { SetHandler(PopupResults.Yes, value); }
	}

	public Handler OnNo
	{
		get { return GetHandler(PopupResults.No); }
		set { SetHandler(PopupResults.No, value); }
	}

	public Handler OnClose
	{
		get { return GetHandler(PopupResults.Close); }
		set { SetHandler(PopupResults.Close, value); }
	}

	Handler GetHandler(PopupResults result)
	{
		Handler handler;
		handlers.TryGetValue(result, out handler);
		return handler;
	}

	void SetHandler(PopupResults result, Handler handler)
	{
		if (State.IsClosed())
		{
			if (result == this.result)
				handler(this);
		}
		else
		{
			handlers[result] = handler;
		}
	}

	Dictionary<PopupResults, Handler> handlers = new Dictionary<PopupResults, Handler>();

	PopupResults result = PopupResults.None;
}

public class PopupState<T> : PopupState
{
	public new T ResultParam 
	{ 
		get { return (T)base.ResultParam; }
		set { base.ResultParam = value; }
	}

	public void SetResult(PopupResults result, T param)
	{
		ResultParam = param;
		Result = result;
	}
}
