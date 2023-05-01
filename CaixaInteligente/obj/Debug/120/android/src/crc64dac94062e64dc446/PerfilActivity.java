package crc64dac94062e64dc446;


public class PerfilActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("CaixaInteligente.PerfilActivity, CaixaInteligente", PerfilActivity.class, __md_methods);
	}


	public PerfilActivity ()
	{
		super ();
		if (getClass () == PerfilActivity.class)
			mono.android.TypeManager.Activate ("CaixaInteligente.PerfilActivity, CaixaInteligente", "", this, new java.lang.Object[] {  });
	}

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
