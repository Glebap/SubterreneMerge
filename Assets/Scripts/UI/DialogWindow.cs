using System;
using UnityEngine.UI;


public class DialogWindow : PopUp
{
	public Button _acceptButton;
	public Button _declineButton;

	public event Action AcceptButtonCLicked;
	public event Action DeclineButtonCLicked;

	
	public void Awake()
	{
		if (_acceptButton != null) 
			_acceptButton.onClick.AddListener(() => AcceptButtonCLicked?.Invoke());
		if (_declineButton != null) 
			_declineButton.onClick.AddListener(() => DeclineButtonCLicked?.Invoke());
	}
}