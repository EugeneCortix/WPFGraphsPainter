   M S B A M L   `   `   `  ????B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089I2clr-namespace:System.Collections;assembly=mscorlibSystem.Collections 5   	       WpfApp1.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:WpfApp1     5   	   Bcol2clr-namespace:System.Collections;assembly=mscorlib     5   	   aPresentationOptionsAhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/options     5      -   5          ?Name$  window??5   	    ? Title$ 
MainWindow??5   	   $	??832??6   $??911.6??6)   .??5      ? $??0,0,0,-6q?6    ? ColumnDefinitions 5   
   ?? 5      $
??127* ?6   ?? 5      $
??329* ?6   .+?5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ dataValuesGrid??5         dataValuesObject#???   6+      AutoGenerateColumns . $??3,12,101,667q?6?   $	??150??6?    K? ZIndex$ 3??6?      ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$ 67 6?   $   HorizontalScrollBarVisibility$ Hidden=?6?    ? 	Resources 5      "  System.Collections.ArrayList   WpfApp1.dataValues	   X
   Y??%?   &	          5        5      $	 0Q?6'   $
 0Q?6-     5      $	 1Q?6'   $
 1Q?6-     5      $	 2Q?6'   $
 4Q?6-   5      5      5      ?? 5   
   -   5        $??	Add Value??5      $
??Left=?6%   $??166,383,0,0q?6@   $	??Top=?6U   $??20??6?   $??60??6?   $ 3??6?   ?? 5   
   -   5        $??
Save As...??5      $
??Left=?6&   $??	9,383,0,0q?6A   $	??Top=?6T   $??20??6?   $??60??6?   $ 3??6?   ?? 5   
   -   5        $??	Open file??5      $
??Left=?6%   $??
84,383,0,0q?6@   $	??Top=?6T   $??20??6l   $??60??6x   $ 3??6?   ?? 5   
   -   5         ??Name$ listOfFiles??5      $??3,194,101,491q?6'   $	??150??6>   $ 3??6m   ?? 5   
   -   5         ??Name$ BoxOfDatasOnGrid??5      $
??Left=?6-   $	??Top=?6H   $	??150??6`   $??	0,167,0,0q?6l   $ 3??6?   ?? 5   
   
???????$??1q?6$   $
??Left=?5       $	??Top=?6    $??	10,10,0,0q?5!      ?? 5"   
   -   5         ??Name$ labelxl??5"      $?? ??6!   $
??Left=?6,   $??247,343,0,0q?6G   $	??Top=?6\   $??2??6t   ?? 5#   
   -	   5        $ labelxm??5#      $?? ??6!   $
??Left=?6,   $??197,343,0,0q?6G   $	??Top=?6\   $??1??6t   ?? 5$   
   -
   5        $ labelxr??5$      $?? ??6!   $
??Left=?6,   $??399,343,0,0q?6G   $	??Top=?6\   $??1??6t   ?? 5%   
   -   5        $ labelyb??5%      $?? ??6!   $
??Left=?6-   $??213,333,0,0q?6H   $	??Top=?6]   ?? 5&   
   -   5        $ labelym??5&      $?? ??6!   $
??Left=?6,   $??218,183,0,0q?6G   $	??Top=?6\   ?? 5'   
   -   5        $ labelyt??5'      $?? ??6!   $
??Left=?6,   $??
218,36,0,0q?6G   $	??Top=?6[   ?? 5(   
   -   5        $ drawWay??5(      $
??Left=?6$   $??	0,346,0,0q?6@   $	??Top=?6S   $	??150??6k   $ 3??6w   6?   ?? 5)   
   -   5         ??Name$ stackLegend??5)      $
??Left=?6*   $	??210??6E   $??
448,99,0,0q?6R   $	??Top=?6f   $	??165??6~   $ 3??6?   $??1??6?   ?? 5*   
   -   5         ??Name$ 
graphImage??5*      $??1??6$   $??2,46,0,0q?64   $	??400??6F   $	??300??6R   $
??Left=?6?   $	??Top=?6?   $
??None=?6?   ?? 5+   
   -   5         ??Name$ 
overCanvas??5+      $
??Left=?6%   $	??403??6@   $	??Top=?6M   $	??252??6e   $ 1??6q   5-      5.      