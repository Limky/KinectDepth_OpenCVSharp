   M S B A M L   `   `   `  ����   DepthBasics-WPF5    /Microsoft.Samples.Kinect.DepthBasics.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     � Title$  Depth Basics��$
��1920��$
��1080�� � WindowState$ 	Maximized=� � 	Resources    MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value  ScreenshotButton �� 
TargetType �� Orientation	 ��Name
 �� TextAlignment ��Name �� Triggers P� Property P� Value �� 
TargetName��%�  &	        &	      &	 *     '�� ?     &	 t     7  7  �� $ 	#ff6e6e6e���� $ 	#ff52318f���� $ 	#ff00BCF2���� " ��.���  �@J�SnapsToDevicePixels . �� " ��.���  � �� �� " ��.R�� .+��� $ 
Horizontal=�
������ .+��� $	 ScreenshotText��#��z��$

 Left=�$��Center=�8��  $��15(�� $��9,0,0,0q�.+��� $ ScreenshotNormal��$!��Images\ScreenshotNormal.png��$
��None=�$��Center=��� $ ScreenshotHover��$ ��Images\ScreenshotHover.png��$
��None=�$��Center=�$y�	Collapsed=� P�  �@J�IsMouseOver . . ��� $ ScreenshotNormal�� � y�$ 	Collapsed=��� $ ScreenshotHover�� � y�$ Visible=��� 8  $ ScreenshotText�� � ��.��� $��	10 0 10 0q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+��� $��0��$��Images\Logo.png��$
��Left=�$
��Fill=�$��32��$��81��$��0 10 0 5q��� $��0��$��0 0 -1 0q�$��Right=�$��Bottom=�#��[  $��Segoe UI)�$��18(��� $�Depth Basics���� $��0��$��Images\Status.png��$
��None=�$��Center=�$��0 0 0 5q�(� $��1��$��Center=�.���� $��Uniform=����� *ImageSource+�� -    ��Name$ 	statusBar��$��3��$��Stretch=�$��Bottom=�
�������#��[  .@��� ���� *
StatusText+	� -    	�Name$ floor_radioButton��$��Floor�� d� 	IsChecked$
 TrueY�$
��Left=�$��	10,29,0,0q�$��2��$	��Top=�	� -   $ wall_radioButton��$
��Wall��$
��Left=�$��	64,29,0,0q�$��2��$	��Top=��� -    ��Name$ 
mode_label��$��Kinect Mode
��$
��Left=�$��10,5,0,0q�$��2��$	��Top=�$��26��$
��Bold"�$��90���� -    ��Name$ findCenter_button��$
��Find��$
��Left=�$��1088,25,0,0q�$��2��$	��Top=�$��75���� -   $ stopCenter_button��$��Stop ��$
��Left=�$��1088,50,0,0q�$��2��$	��Top=�$��75�� J� RenderTransformOrigin$ 0.216,0.091&��� -    ��Name$ minDepth_textBox��$
��Left=�$��23��$��
174,18,0,0q�$��2�� �� TextWrapping$
 Wrap=�$��0��$	��Top=�$��42���� -	   $ minDepth_label��$��	MinDepth
��$
��Left=�$��
112,17,0,0q�$��2��$	��Top=�$��28���� -
   $ maxDepth_label��$��MaxDepth��$
��Left=�$��
111,42,0,7q�$��2���� -   $ maxDepth_textBox��$
��Left=�$��23��$��
174,46,0,0q�$��2��$
 Wrap=�$
��1837��$	��Top=�$��42���� -    ��Name$ 
LayoutRoot��$
��Left=�$	��Top=��� -   $ 	cog_label��$��Floor-Center��$
��Left=�$��
1084,1,0,0q�$��2��$	��Top=�$
��Bold"��� -    ��Name$ reversePoint_checkBox��$��Reverse��$
��Left=�$��	23,51,0,0q�$��2��$	��Top=��� -   $ saveConfig_button��$��
Local Save��$
��Left=�$��1734,28,0,0q�$��2��$��75��$��20��$	��Top=��� -   $ readConfig_button��$��
Local Load��$
��Left=�$��1734,52,0,3q�$��2��$��75���� -   $ label��$
��Left��$
��Left=�$��683,24,0,25q�$��2��$ 	1.5,0.538&��� -   $ cog_label_Copy��$��Floor-Padding-Detection��$
��Left=�$��	716,3,0,0q�$��2��$	��Top=�$
��Bold"�$��27���� -   $ label1��$��Right��$
��Left=�$��
675,47,0,0q�$��2��$	��Top=�$ 0.496,0.115&��� -   $  left_padding_width_textBox��$
��Left=�$��23��$��
718,26,0,0q�$��2��$
 Wrap=�$��0��$	��Top=�$��52���� -   $! right_padding_width_textBox��$
��Left=�$��23��$��
718,51,0,0q�$��2��$
 Wrap=�$��0��$	��Top=�$��52���� -   $ 
label_Copy��$	��Top��$
��Left=�$��
793,24,0,0q�$��2��$	��Top=��� -   $ label1_Copy��$��Bottom��$
��Left=�$��
774,47,0,0q�$��2��$	��Top=�$ 0.496,0.115&��� -   $  top_padding_height_textBox��$
��Left=�$��23��$��
828,26,0,0q�$��2��$
 Wrap=�$��0��$	��Top=�$��52���� -   $# bottom_padding_height_textBox��$
��Left=�$��23��$��
828,51,0,0q�$��2��$
 Wrap=�$��0��$	��Top=�$��52���� -   $ label2��$��Start-X��$
��Left=�$��
895,24,0,0q�$��2��$	��Top=��� -   $ label2_Copy��$��Start-Y��$
��Left=�$��
895,47,0,0q�$��2��$	��Top=��� -   $ startX_textBox��$
��Left=�$��23��$��
941,25,0,0q�$��2��$
 Wrap=�$��0��$	��Top=�$��52���� -   $ startY_textBox��$
��Left=�$��23��$��
941,50,0,0q�$��2��$
 Wrap=�$��0��$	��Top=�$��52���� -   $ cog_label_Copy1��$��Floor-Starting-X Y��$
��Left=�$��	891,2,0,0q�$��2��$	��Top=�$
��Bold"��� -   $ label2_Copy1��$��width��$
��Left=�$��
228,20,0,0q�$��2��$	��Top=�$y�Hidden=��� -    $ label2_Copy2��$��Height��$
��Left=�$��
221,42,0,0q�$��2��$	��Top=�$y�Hidden=��� -!   $ cog_label_Copy2��$��Unity-Screen��$
��Left=�$��	228,1,0,0q�$��2��$	��Top=�$
��Bold"�$y�Hidden=��� -"   $ unity_width_textBox��$
��Left=�$��23��$��
266,22,0,0q�$��2��$
 Wrap=�$
��1330��$	��Top=�$��52��$y�Hidden=��� -#   $ unity_height_textBox��$
��Left=�$��23��$��
266,46,0,0q�$��2��$
 Wrap=�$
��1000��$	��Top=�$��52��$y�Hidden=��� -$   $ startY_label_Copy��$��wall-Y��$
��Left=�$��1169,17,0,0q�$��2��$	��Top=��� -%   $ cog_label_Copy3��$��Wall-Detection��$
��Left=�$��
1189,0,0,0q�$��2��$	��Top=�$
��Bold"��� -&   $ wall_startY_textBox��$
��Left=�$��23��$��1215,20,0,0q�$��2��$
 Wrap=�$	��250��$	��Top=�$��43���� -'   $ server_IP_label��$��IP��$
��Left=�$��
220,17,0,0q�$��2��$	��Top=�$
��Bold"��� -(   $ server_Port_label��$
��Port��$
��Left=�$��
220,42,0,0q�$��2��$	��Top=�$
��Bold"��� -)   $ server_IPaddress_Octet_01��$
��Left=�$��23��$��
252,20,0,0q�$��2��$
 Wrap=�$	��192��$	��Top=�$��33���� -*   $ server_IPaddress_Octet_02��$
��Left=�$��23��$��
290,20,0,0q�$��2��$
 Wrap=�$	��168��$	��Top=�$��33���� -+   $ server_IPaddress_Octet_03��$
��Left=�$��23��$��
328,20,0,0q�$��2��$
 Wrap=�$��2��$	��Top=�$��33���� -,   $ server_IPaddress_Octet_04��$
��Left=�$��23��$��
366,20,0,0q�$��2��$
 Wrap=�$	��200��$	��Top=�$��33���� --   $ server_Port_Number��$
��Left=�$��23��$��
252,46,0,0q�$��2��$
 Wrap=�$
��8080��$	��Top=�$��33���� -.   $ server_IP_save_button��$
��Save��$
��Left=�$��
485,49,0,6q�$��2��$��64���� -/   $ 	xml_label��$	��XML��$
��Left=�$��
1734,1,0,0q�$��2��$	��Top=�$
��Bold"�$ 0.5,0.5&�z�V� .~��� �� ��  �� Angle$ -0.796Q�T� �� -0    ��Name$ gridSplitter��$
��Left=�$��78��$��	671,3,0,0q�$��2��$	��Top=�$��2��$��2���� -1   $ gridSplitter_Copy��$
��Left=�$��73��$��
1167,3,0,0q�$��2��$	��Top=�$��2��$��2���� -2   $ gridSplitter_Copy1��$
��Left=�$��73��$��
1541,5,0,0q�$��2��$	��Top=�$��2��$��2���� -3   $ unity_connect_status��$��Server info..��$
��Left=�$��
560,19,0,0q�$��2��$	��Top=�$
��Bold"��� -4   $ startY_label_Copy1��$��scale��$
��Left=�$��1170,41,0,0q�$��2��$	��Top=��� -5    ��Name$ wall_scale_slider��$
��Left=�$��1203,47,0,0q�$��2��$	��Top=�$��37�� � LargeChange$ 0Q� � SmallChange$
 0.01Q��� -6   $ wall_scale_value_label��$��S.��$
��Left=�$��1236,42,0,0q�$��2��$	��Top=��� -7   $ sending_to_unity_XY��$��Status..X Y��$
��Left=�$��
833,12,0,0q�$��2��$	��Top=�$
��Bold"�$��78��$��44��$y�Hidden=��� -8   $ x_detection_textBox��$
��Left=�$��23��$��1435,23,0,0q�$��2��$
 Wrap=�$��70��$	��Top=�$��34���� -9   $ startY_label_Copy2��$��X��$
��Left=�$��1418,20,0,0q�$��2��$	��Top=��� -:   $ y_detection_textBox��$
��Left=�$��23��$��1435,48,0,0q�$��2��$
 Wrap=�$	��100��$	��Top=�$��34���� -;   $ startY_label_Copy3��$��Y��$
��Left=�$��1418,45,0,0q�$��2��$	��Top=��� -<   $ cog_label_Copy4��$��Wall-XY-Detection��$
��Left=�$��
1416,3,0,0q�$��2��$	��Top=�$
��Bold"��� -=   $ label3��$��Target��$
��Left=�$��
286,43,0,0q�$��2��$	��Top=�$
��Bold"��� ->   $ target_text_box��$
��Left=�$��23��$��
331,48,0,0q�$��2��$
 Wrap=�$��STEP021��$	��Top=�$��68���� -?   $ saveServerConfig_button��$��Server Save��$
��Left=�$��1814,28,0,0q�$��2��$	��Top=�$��75���� -@   $ loadServerConfig_button��$��Server Load��$
��Left=�$��1814,52,0,0q�$��2��$	��Top=�$��75���� -A   $ nettype_textBox��$
��Left=�$��23��$��
454,48,0,0q�$��2��$
 Wrap=�$��0��$	��Top=�$��26���� -B   $ label3_Copy��$��Nettype��$
��Left=�$��
401,45,0,0q�$��2��$	��Top=�$
��Bold"��� -C   $ server_IP_label_Copy��$��Server��$
��Left=�$��	220,0,0,0q�$��2��$	��Top=�$
��Bold"��� -D   $ label3_Copy1��$��
DeviceCode��$
��Left=�$��
399,19,0,0q�$��2��$	��Top=�$
��Bold"��� -E   $ deviceCode_textBox��$
��Left=�$��23��$��
476,21,0,0q�$��2��$
 Wrap=�$��	SMA-60000��$	��Top=�$��73���� -F   $ frame_sec_textBox��$
��Left=�$��23��$��1029,26,0,0q�$��2��$
 Wrap=�$	��0.5��$	��Top=�$��31���� -G   $ cog_label_Copy5��$
��Ofps��$
��Left=�$��
1025,0,0,0q�$��2��$	��Top=�$
��Bold"��� -H   $ label2_Copy3��$��S��$
��Left=�$��1012,23,0,0q�$��2��$	��Top=��� -I   $ startY_label_Copy4��$��wall-L��$
��Left=�$��1274,17,0,0q�$��2��$	��Top=��� -J   $ left_wall_scale_slider��$
��Left=�$��1316,22,0,0q�$��2��$	��Top=�$��50��$
 0.01Q�$ 0Q��� -K   $ startY_label_Copy5��$��wall-R��$
��Left=�$��1274,42,0,0q�$��2��$	��Top=��� -L   $ right_wall_scale_slider��$
��Left=�$��1316,45,0,0q�$��2��$	��Top=�$��50��$
 0.01Q�$ 0Q��� -M   $! left_wall_scale_value_label��$��L-value��$
��Left=�$��1359,16,0,0q�$��2��$	��Top=��� -N   $" right_wall_scale_value_label��$��R-value��$
��Left=�$��1359,42,0,0q�$��2��$	��Top=�