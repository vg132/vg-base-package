# VG Software base package for Unity projects

## Script AddNamespace

This editor plugin will try to create a namespace based on the folder structure starting with Assets in C# files.
For this plugin to work you need to update the Unity template for the C# script files, on Windows they are found here,
`C:\Program Files\Unity\Hub\Editor\[UNITY_VERSION]\Editor\Data\Resources\ScriptTemplates`, but my be different on your machine based on where you have Unity installed.

The updates needed are that `#ROOTNAMESPACEBEGIN#` and `#ROOTNAMESPACEEND#` needs to be removed and `namespace #NAMESPACE#` needs to be added to the top and `}` to the bottom of the template
Here is an example of a full template

```
using UnityEngine;

namespace #NAMESPACE#
{
	public class #SCRIPTNAME# : MonoBehaviour
	{
	}
}
```

## Script AutoRefresher

This script will monitor your projects `Assets` folder for changes in C# (\*.cs) files and if any changes are detected then it will recompile the project the next time `PlayMode` is entered, this in combination with turning off Unity's built in "Autorefresh" feature will improve the workflow. You can also trigger a rebuild using the new build option `Refresh asset database` in the Files menu.
