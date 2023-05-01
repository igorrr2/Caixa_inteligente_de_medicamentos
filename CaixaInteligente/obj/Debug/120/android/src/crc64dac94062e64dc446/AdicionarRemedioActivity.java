package crc64dac94062e64dc446;


public class AdicionarRemedioActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("CaixaInteligente.AdicionarRemedioActivity, CaixaInteligente", AdicionarRemedioActivity.class, __md_methods);
	}


	public AdicionarRemedioActivity ()
	{
		super ();
		if (getClass () == AdicionarRemedioActivity.class)
			mono.android.TypeManager.Activate ("CaixaInteligente.AdicionarRemedioActivity, CaixaInteligente", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
