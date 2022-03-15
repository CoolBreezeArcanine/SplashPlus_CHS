namespace DB
{
	public static class KeyCodeIDEnum
	{
		private static readonly KeyCodeTableRecord[] records = new KeyCodeTableRecord[321]
		{
			new KeyCodeTableRecord(0, "None", "None", 0),
			new KeyCodeTableRecord(1, "Backspace", "Backspace", 8),
			new KeyCodeTableRecord(2, "Tab", "Tab", 9),
			new KeyCodeTableRecord(3, "Clear", "Clear", 12),
			new KeyCodeTableRecord(4, "Return", "Return", 13),
			new KeyCodeTableRecord(5, "Pause", "Pause", 19),
			new KeyCodeTableRecord(6, "Escape", "Escape", 27),
			new KeyCodeTableRecord(7, "Space", "Space", 32),
			new KeyCodeTableRecord(8, "Exclaim", "Exclaim", 33),
			new KeyCodeTableRecord(9, "DoubleQuote", "DoubleQuote", 34),
			new KeyCodeTableRecord(10, "Hash", "Hash", 35),
			new KeyCodeTableRecord(11, "Dollar", "Dollar", 36),
			new KeyCodeTableRecord(12, "Ampersand", "Ampersand", 38),
			new KeyCodeTableRecord(13, "Quote", "Quote", 39),
			new KeyCodeTableRecord(14, "LeftParen", "LeftParen", 40),
			new KeyCodeTableRecord(15, "RightParen", "RightParen", 41),
			new KeyCodeTableRecord(16, "Asterisk", "Asterisk", 42),
			new KeyCodeTableRecord(17, "Plus", "Plus", 43),
			new KeyCodeTableRecord(18, "Comma", "Comma", 44),
			new KeyCodeTableRecord(19, "Minus", "Minus", 45),
			new KeyCodeTableRecord(20, "Period", "Period", 46),
			new KeyCodeTableRecord(21, "Slash", "Slash", 47),
			new KeyCodeTableRecord(22, "Alpha0", "Alpha0", 48),
			new KeyCodeTableRecord(23, "Alpha1", "Alpha1", 49),
			new KeyCodeTableRecord(24, "Alpha2", "Alpha2", 50),
			new KeyCodeTableRecord(25, "Alpha3", "Alpha3", 51),
			new KeyCodeTableRecord(26, "Alpha4", "Alpha4", 52),
			new KeyCodeTableRecord(27, "Alpha5", "Alpha5", 53),
			new KeyCodeTableRecord(28, "Alpha6", "Alpha6", 54),
			new KeyCodeTableRecord(29, "Alpha7", "Alpha7", 55),
			new KeyCodeTableRecord(30, "Alpha8", "Alpha8", 56),
			new KeyCodeTableRecord(31, "Alpha9", "Alpha9", 57),
			new KeyCodeTableRecord(32, "Colon", "Colon", 58),
			new KeyCodeTableRecord(33, "Semicolon", "Semicolon", 59),
			new KeyCodeTableRecord(34, "Less", "Less", 60),
			new KeyCodeTableRecord(35, "Equals", "Equals", 61),
			new KeyCodeTableRecord(36, "Greater", "Greater", 62),
			new KeyCodeTableRecord(37, "Question", "Question", 63),
			new KeyCodeTableRecord(38, "At", "At", 64),
			new KeyCodeTableRecord(39, "LeftBracket", "LeftBracket", 91),
			new KeyCodeTableRecord(40, "Backslash", "Backslash", 92),
			new KeyCodeTableRecord(41, "RightBracket", "RightBracket", 93),
			new KeyCodeTableRecord(42, "Caret", "Caret", 94),
			new KeyCodeTableRecord(43, "Underscore", "Underscore", 95),
			new KeyCodeTableRecord(44, "BackQuote", "BackQuote", 96),
			new KeyCodeTableRecord(45, "A", "A", 97),
			new KeyCodeTableRecord(46, "B", "B", 98),
			new KeyCodeTableRecord(47, "C", "C", 99),
			new KeyCodeTableRecord(48, "D", "D", 100),
			new KeyCodeTableRecord(49, "E", "E", 101),
			new KeyCodeTableRecord(50, "F", "F", 102),
			new KeyCodeTableRecord(51, "G", "G", 103),
			new KeyCodeTableRecord(52, "H", "H", 104),
			new KeyCodeTableRecord(53, "I", "I", 105),
			new KeyCodeTableRecord(54, "J", "J", 106),
			new KeyCodeTableRecord(55, "K", "K", 107),
			new KeyCodeTableRecord(56, "L", "L", 108),
			new KeyCodeTableRecord(57, "M", "M", 109),
			new KeyCodeTableRecord(58, "N", "N", 110),
			new KeyCodeTableRecord(59, "O", "O", 111),
			new KeyCodeTableRecord(60, "P", "P", 112),
			new KeyCodeTableRecord(61, "Q", "Q", 113),
			new KeyCodeTableRecord(62, "R", "R", 114),
			new KeyCodeTableRecord(63, "S", "S", 115),
			new KeyCodeTableRecord(64, "T", "T", 116),
			new KeyCodeTableRecord(65, "U", "U", 117),
			new KeyCodeTableRecord(66, "V", "V", 118),
			new KeyCodeTableRecord(67, "W", "W", 119),
			new KeyCodeTableRecord(68, "X", "X", 120),
			new KeyCodeTableRecord(69, "Y", "Y", 121),
			new KeyCodeTableRecord(70, "Z", "Z", 122),
			new KeyCodeTableRecord(71, "Delete", "Delete", 127),
			new KeyCodeTableRecord(72, "Keypad0", "Keypad0", 256),
			new KeyCodeTableRecord(73, "Keypad1", "Keypad1", 257),
			new KeyCodeTableRecord(74, "Keypad2", "Keypad2", 258),
			new KeyCodeTableRecord(75, "Keypad3", "Keypad3", 259),
			new KeyCodeTableRecord(76, "Keypad4", "Keypad4", 260),
			new KeyCodeTableRecord(77, "Keypad5", "Keypad5", 261),
			new KeyCodeTableRecord(78, "Keypad6", "Keypad6", 262),
			new KeyCodeTableRecord(79, "Keypad7", "Keypad7", 263),
			new KeyCodeTableRecord(80, "Keypad8", "Keypad8", 264),
			new KeyCodeTableRecord(81, "Keypad9", "Keypad9", 265),
			new KeyCodeTableRecord(82, "KeypadPeriod", "KeypadPeriod", 266),
			new KeyCodeTableRecord(83, "KeypadDivide", "KeypadDivide", 267),
			new KeyCodeTableRecord(84, "KeypadMultiply", "KeypadMultiply", 268),
			new KeyCodeTableRecord(85, "KeypadMinus", "KeypadMinus", 269),
			new KeyCodeTableRecord(86, "KeypadPlus", "KeypadPlus", 270),
			new KeyCodeTableRecord(87, "KeypadEnter", "KeypadEnter", 271),
			new KeyCodeTableRecord(88, "KeypadEquals", "KeypadEquals", 272),
			new KeyCodeTableRecord(89, "UpArrow", "UpArrow", 273),
			new KeyCodeTableRecord(90, "DownArrow", "DownArrow", 274),
			new KeyCodeTableRecord(91, "RightArrow", "RightArrow", 275),
			new KeyCodeTableRecord(92, "LeftArrow", "LeftArrow", 276),
			new KeyCodeTableRecord(93, "Insert", "Insert", 277),
			new KeyCodeTableRecord(94, "Home", "Home", 278),
			new KeyCodeTableRecord(95, "End_", "End", 279),
			new KeyCodeTableRecord(96, "PageUp", "PageUp", 280),
			new KeyCodeTableRecord(97, "PageDown", "PageDown", 281),
			new KeyCodeTableRecord(98, "F1", "F1", 282),
			new KeyCodeTableRecord(99, "F2", "F2", 283),
			new KeyCodeTableRecord(100, "F3", "F3", 284),
			new KeyCodeTableRecord(101, "F4", "F4", 285),
			new KeyCodeTableRecord(102, "F5", "F5", 286),
			new KeyCodeTableRecord(103, "F6", "F6", 287),
			new KeyCodeTableRecord(104, "F7", "F7", 288),
			new KeyCodeTableRecord(105, "F8", "F8", 289),
			new KeyCodeTableRecord(106, "F9", "F9", 290),
			new KeyCodeTableRecord(107, "F10", "F10", 291),
			new KeyCodeTableRecord(108, "F11", "F11", 292),
			new KeyCodeTableRecord(109, "F12", "F12", 293),
			new KeyCodeTableRecord(110, "F13", "F13", 294),
			new KeyCodeTableRecord(111, "F14", "F14", 295),
			new KeyCodeTableRecord(112, "F15", "F15", 296),
			new KeyCodeTableRecord(113, "Numlock", "Numlock", 300),
			new KeyCodeTableRecord(114, "CapsLock", "CapsLock", 301),
			new KeyCodeTableRecord(115, "ScrollLock", "ScrollLock", 302),
			new KeyCodeTableRecord(116, "RightShift", "RightShift", 303),
			new KeyCodeTableRecord(117, "LeftShift", "LeftShift", 304),
			new KeyCodeTableRecord(118, "RightControl", "RightControl", 305),
			new KeyCodeTableRecord(119, "LeftControl", "LeftControl", 306),
			new KeyCodeTableRecord(120, "RightAlt", "RightAlt", 307),
			new KeyCodeTableRecord(121, "LeftAlt", "LeftAlt", 308),
			new KeyCodeTableRecord(122, "RightCommand", "RightCommand", 309),
			new KeyCodeTableRecord(123, "RightApple", "RightApple", 309),
			new KeyCodeTableRecord(124, "LeftCommand", "LeftCommand", 310),
			new KeyCodeTableRecord(125, "LeftApple", "LeftApple", 310),
			new KeyCodeTableRecord(126, "LeftWindows", "LeftWindows", 311),
			new KeyCodeTableRecord(127, "RightWindows", "RightWindows", 312),
			new KeyCodeTableRecord(128, "AltGr", "AltGr", 313),
			new KeyCodeTableRecord(129, "Help", "Help", 315),
			new KeyCodeTableRecord(130, "Print", "Print", 316),
			new KeyCodeTableRecord(131, "SysReq", "SysReq", 317),
			new KeyCodeTableRecord(132, "Break", "Break", 318),
			new KeyCodeTableRecord(133, "Menu", "Menu", 319),
			new KeyCodeTableRecord(134, "Mouse0", "Mouse0", 323),
			new KeyCodeTableRecord(135, "Mouse1", "Mouse1", 324),
			new KeyCodeTableRecord(136, "Mouse2", "Mouse2", 325),
			new KeyCodeTableRecord(137, "Mouse3", "Mouse3", 326),
			new KeyCodeTableRecord(138, "Mouse4", "Mouse4", 327),
			new KeyCodeTableRecord(139, "Mouse5", "Mouse5", 328),
			new KeyCodeTableRecord(140, "Mouse6", "Mouse6", 329),
			new KeyCodeTableRecord(141, "JoystickButton0", "JoystickButton0", 330),
			new KeyCodeTableRecord(142, "JoystickButton1", "JoystickButton1", 331),
			new KeyCodeTableRecord(143, "JoystickButton2", "JoystickButton2", 332),
			new KeyCodeTableRecord(144, "JoystickButton3", "JoystickButton3", 333),
			new KeyCodeTableRecord(145, "JoystickButton4", "JoystickButton4", 334),
			new KeyCodeTableRecord(146, "JoystickButton5", "JoystickButton5", 335),
			new KeyCodeTableRecord(147, "JoystickButton6", "JoystickButton6", 336),
			new KeyCodeTableRecord(148, "JoystickButton7", "JoystickButton7", 337),
			new KeyCodeTableRecord(149, "JoystickButton8", "JoystickButton8", 338),
			new KeyCodeTableRecord(150, "JoystickButton9", "JoystickButton9", 339),
			new KeyCodeTableRecord(151, "JoystickButton10", "JoystickButton10", 340),
			new KeyCodeTableRecord(152, "JoystickButton11", "JoystickButton11", 341),
			new KeyCodeTableRecord(153, "JoystickButton12", "JoystickButton12", 342),
			new KeyCodeTableRecord(154, "JoystickButton13", "JoystickButton13", 343),
			new KeyCodeTableRecord(155, "JoystickButton14", "JoystickButton14", 344),
			new KeyCodeTableRecord(156, "JoystickButton15", "JoystickButton15", 345),
			new KeyCodeTableRecord(157, "JoystickButton16", "JoystickButton16", 346),
			new KeyCodeTableRecord(158, "JoystickButton17", "JoystickButton17", 347),
			new KeyCodeTableRecord(159, "JoystickButton18", "JoystickButton18", 348),
			new KeyCodeTableRecord(160, "JoystickButton19", "JoystickButton19", 349),
			new KeyCodeTableRecord(161, "Joystick1Button0", "Joystick1Button0", 350),
			new KeyCodeTableRecord(162, "Joystick1Button1", "Joystick1Button1", 351),
			new KeyCodeTableRecord(163, "Joystick1Button2", "Joystick1Button2", 352),
			new KeyCodeTableRecord(164, "Joystick1Button3", "Joystick1Button3", 353),
			new KeyCodeTableRecord(165, "Joystick1Button4", "Joystick1Button4", 354),
			new KeyCodeTableRecord(166, "Joystick1Button5", "Joystick1Button5", 355),
			new KeyCodeTableRecord(167, "Joystick1Button6", "Joystick1Button6", 356),
			new KeyCodeTableRecord(168, "Joystick1Button7", "Joystick1Button7", 357),
			new KeyCodeTableRecord(169, "Joystick1Button8", "Joystick1Button8", 358),
			new KeyCodeTableRecord(170, "Joystick1Button9", "Joystick1Button9", 359),
			new KeyCodeTableRecord(171, "Joystick1Button10", "Joystick1Button10", 360),
			new KeyCodeTableRecord(172, "Joystick1Button11", "Joystick1Button11", 361),
			new KeyCodeTableRecord(173, "Joystick1Button12", "Joystick1Button12", 362),
			new KeyCodeTableRecord(174, "Joystick1Button13", "Joystick1Button13", 363),
			new KeyCodeTableRecord(175, "Joystick1Button14", "Joystick1Button14", 364),
			new KeyCodeTableRecord(176, "Joystick1Button15", "Joystick1Button15", 365),
			new KeyCodeTableRecord(177, "Joystick1Button16", "Joystick1Button16", 366),
			new KeyCodeTableRecord(178, "Joystick1Button17", "Joystick1Button17", 367),
			new KeyCodeTableRecord(179, "Joystick1Button18", "Joystick1Button18", 368),
			new KeyCodeTableRecord(180, "Joystick1Button19", "Joystick1Button19", 369),
			new KeyCodeTableRecord(181, "Joystick2Button0", "Joystick2Button0", 370),
			new KeyCodeTableRecord(182, "Joystick2Button1", "Joystick2Button1", 371),
			new KeyCodeTableRecord(183, "Joystick2Button2", "Joystick2Button2", 372),
			new KeyCodeTableRecord(184, "Joystick2Button3", "Joystick2Button3", 373),
			new KeyCodeTableRecord(185, "Joystick2Button4", "Joystick2Button4", 374),
			new KeyCodeTableRecord(186, "Joystick2Button5", "Joystick2Button5", 375),
			new KeyCodeTableRecord(187, "Joystick2Button6", "Joystick2Button6", 376),
			new KeyCodeTableRecord(188, "Joystick2Button7", "Joystick2Button7", 377),
			new KeyCodeTableRecord(189, "Joystick2Button8", "Joystick2Button8", 378),
			new KeyCodeTableRecord(190, "Joystick2Button9", "Joystick2Button9", 379),
			new KeyCodeTableRecord(191, "Joystick2Button10", "Joystick2Button10", 380),
			new KeyCodeTableRecord(192, "Joystick2Button11", "Joystick2Button11", 381),
			new KeyCodeTableRecord(193, "Joystick2Button12", "Joystick2Button12", 382),
			new KeyCodeTableRecord(194, "Joystick2Button13", "Joystick2Button13", 383),
			new KeyCodeTableRecord(195, "Joystick2Button14", "Joystick2Button14", 384),
			new KeyCodeTableRecord(196, "Joystick2Button15", "Joystick2Button15", 385),
			new KeyCodeTableRecord(197, "Joystick2Button16", "Joystick2Button16", 386),
			new KeyCodeTableRecord(198, "Joystick2Button17", "Joystick2Button17", 387),
			new KeyCodeTableRecord(199, "Joystick2Button18", "Joystick2Button18", 388),
			new KeyCodeTableRecord(200, "Joystick2Button19", "Joystick2Button19", 389),
			new KeyCodeTableRecord(201, "Joystick3Button0", "Joystick3Button0", 390),
			new KeyCodeTableRecord(202, "Joystick3Button1", "Joystick3Button1", 391),
			new KeyCodeTableRecord(203, "Joystick3Button2", "Joystick3Button2", 392),
			new KeyCodeTableRecord(204, "Joystick3Button3", "Joystick3Button3", 393),
			new KeyCodeTableRecord(205, "Joystick3Button4", "Joystick3Button4", 394),
			new KeyCodeTableRecord(206, "Joystick3Button5", "Joystick3Button5", 395),
			new KeyCodeTableRecord(207, "Joystick3Button6", "Joystick3Button6", 396),
			new KeyCodeTableRecord(208, "Joystick3Button7", "Joystick3Button7", 397),
			new KeyCodeTableRecord(209, "Joystick3Button8", "Joystick3Button8", 398),
			new KeyCodeTableRecord(210, "Joystick3Button9", "Joystick3Button9", 399),
			new KeyCodeTableRecord(211, "Joystick3Button10", "Joystick3Button10", 400),
			new KeyCodeTableRecord(212, "Joystick3Button11", "Joystick3Button11", 401),
			new KeyCodeTableRecord(213, "Joystick3Button12", "Joystick3Button12", 402),
			new KeyCodeTableRecord(214, "Joystick3Button13", "Joystick3Button13", 403),
			new KeyCodeTableRecord(215, "Joystick3Button14", "Joystick3Button14", 404),
			new KeyCodeTableRecord(216, "Joystick3Button15", "Joystick3Button15", 405),
			new KeyCodeTableRecord(217, "Joystick3Button16", "Joystick3Button16", 406),
			new KeyCodeTableRecord(218, "Joystick3Button17", "Joystick3Button17", 407),
			new KeyCodeTableRecord(219, "Joystick3Button18", "Joystick3Button18", 408),
			new KeyCodeTableRecord(220, "Joystick3Button19", "Joystick3Button19", 409),
			new KeyCodeTableRecord(221, "Joystick4Button0", "Joystick4Button0", 410),
			new KeyCodeTableRecord(222, "Joystick4Button1", "Joystick4Button1", 411),
			new KeyCodeTableRecord(223, "Joystick4Button2", "Joystick4Button2", 412),
			new KeyCodeTableRecord(224, "Joystick4Button3", "Joystick4Button3", 413),
			new KeyCodeTableRecord(225, "Joystick4Button4", "Joystick4Button4", 414),
			new KeyCodeTableRecord(226, "Joystick4Button5", "Joystick4Button5", 415),
			new KeyCodeTableRecord(227, "Joystick4Button6", "Joystick4Button6", 416),
			new KeyCodeTableRecord(228, "Joystick4Button7", "Joystick4Button7", 417),
			new KeyCodeTableRecord(229, "Joystick4Button8", "Joystick4Button8", 418),
			new KeyCodeTableRecord(230, "Joystick4Button9", "Joystick4Button9", 419),
			new KeyCodeTableRecord(231, "Joystick4Button10", "Joystick4Button10", 420),
			new KeyCodeTableRecord(232, "Joystick4Button11", "Joystick4Button11", 421),
			new KeyCodeTableRecord(233, "Joystick4Button12", "Joystick4Button12", 422),
			new KeyCodeTableRecord(234, "Joystick4Button13", "Joystick4Button13", 423),
			new KeyCodeTableRecord(235, "Joystick4Button14", "Joystick4Button14", 424),
			new KeyCodeTableRecord(236, "Joystick4Button15", "Joystick4Button15", 425),
			new KeyCodeTableRecord(237, "Joystick4Button16", "Joystick4Button16", 426),
			new KeyCodeTableRecord(238, "Joystick4Button17", "Joystick4Button17", 427),
			new KeyCodeTableRecord(239, "Joystick4Button18", "Joystick4Button18", 428),
			new KeyCodeTableRecord(240, "Joystick4Button19", "Joystick4Button19", 429),
			new KeyCodeTableRecord(241, "Joystick5Button0", "Joystick5Button0", 430),
			new KeyCodeTableRecord(242, "Joystick5Button1", "Joystick5Button1", 431),
			new KeyCodeTableRecord(243, "Joystick5Button2", "Joystick5Button2", 432),
			new KeyCodeTableRecord(244, "Joystick5Button3", "Joystick5Button3", 433),
			new KeyCodeTableRecord(245, "Joystick5Button4", "Joystick5Button4", 434),
			new KeyCodeTableRecord(246, "Joystick5Button5", "Joystick5Button5", 435),
			new KeyCodeTableRecord(247, "Joystick5Button6", "Joystick5Button6", 436),
			new KeyCodeTableRecord(248, "Joystick5Button7", "Joystick5Button7", 437),
			new KeyCodeTableRecord(249, "Joystick5Button8", "Joystick5Button8", 438),
			new KeyCodeTableRecord(250, "Joystick5Button9", "Joystick5Button9", 439),
			new KeyCodeTableRecord(251, "Joystick5Button10", "Joystick5Button10", 440),
			new KeyCodeTableRecord(252, "Joystick5Button11", "Joystick5Button11", 441),
			new KeyCodeTableRecord(253, "Joystick5Button12", "Joystick5Button12", 442),
			new KeyCodeTableRecord(254, "Joystick5Button13", "Joystick5Button13", 443),
			new KeyCodeTableRecord(255, "Joystick5Button14", "Joystick5Button14", 444),
			new KeyCodeTableRecord(256, "Joystick5Button15", "Joystick5Button15", 445),
			new KeyCodeTableRecord(257, "Joystick5Button16", "Joystick5Button16", 446),
			new KeyCodeTableRecord(258, "Joystick5Button17", "Joystick5Button17", 447),
			new KeyCodeTableRecord(259, "Joystick5Button18", "Joystick5Button18", 448),
			new KeyCodeTableRecord(260, "Joystick5Button19", "Joystick5Button19", 449),
			new KeyCodeTableRecord(261, "Joystick6Button0", "Joystick6Button0", 450),
			new KeyCodeTableRecord(262, "Joystick6Button1", "Joystick6Button1", 451),
			new KeyCodeTableRecord(263, "Joystick6Button2", "Joystick6Button2", 452),
			new KeyCodeTableRecord(264, "Joystick6Button3", "Joystick6Button3", 453),
			new KeyCodeTableRecord(265, "Joystick6Button4", "Joystick6Button4", 454),
			new KeyCodeTableRecord(266, "Joystick6Button5", "Joystick6Button5", 455),
			new KeyCodeTableRecord(267, "Joystick6Button6", "Joystick6Button6", 456),
			new KeyCodeTableRecord(268, "Joystick6Button7", "Joystick6Button7", 457),
			new KeyCodeTableRecord(269, "Joystick6Button8", "Joystick6Button8", 458),
			new KeyCodeTableRecord(270, "Joystick6Button9", "Joystick6Button9", 459),
			new KeyCodeTableRecord(271, "Joystick6Button10", "Joystick6Button10", 460),
			new KeyCodeTableRecord(272, "Joystick6Button11", "Joystick6Button11", 461),
			new KeyCodeTableRecord(273, "Joystick6Button12", "Joystick6Button12", 462),
			new KeyCodeTableRecord(274, "Joystick6Button13", "Joystick6Button13", 463),
			new KeyCodeTableRecord(275, "Joystick6Button14", "Joystick6Button14", 464),
			new KeyCodeTableRecord(276, "Joystick6Button15", "Joystick6Button15", 465),
			new KeyCodeTableRecord(277, "Joystick6Button16", "Joystick6Button16", 466),
			new KeyCodeTableRecord(278, "Joystick6Button17", "Joystick6Button17", 467),
			new KeyCodeTableRecord(279, "Joystick6Button18", "Joystick6Button18", 468),
			new KeyCodeTableRecord(280, "Joystick6Button19", "Joystick6Button19", 469),
			new KeyCodeTableRecord(281, "Joystick7Button0", "Joystick7Button0", 470),
			new KeyCodeTableRecord(282, "Joystick7Button1", "Joystick7Button1", 471),
			new KeyCodeTableRecord(283, "Joystick7Button2", "Joystick7Button2", 472),
			new KeyCodeTableRecord(284, "Joystick7Button3", "Joystick7Button3", 473),
			new KeyCodeTableRecord(285, "Joystick7Button4", "Joystick7Button4", 474),
			new KeyCodeTableRecord(286, "Joystick7Button5", "Joystick7Button5", 475),
			new KeyCodeTableRecord(287, "Joystick7Button6", "Joystick7Button6", 476),
			new KeyCodeTableRecord(288, "Joystick7Button7", "Joystick7Button7", 477),
			new KeyCodeTableRecord(289, "Joystick7Button8", "Joystick7Button8", 478),
			new KeyCodeTableRecord(290, "Joystick7Button9", "Joystick7Button9", 479),
			new KeyCodeTableRecord(291, "Joystick7Button10", "Joystick7Button10", 480),
			new KeyCodeTableRecord(292, "Joystick7Button11", "Joystick7Button11", 481),
			new KeyCodeTableRecord(293, "Joystick7Button12", "Joystick7Button12", 482),
			new KeyCodeTableRecord(294, "Joystick7Button13", "Joystick7Button13", 483),
			new KeyCodeTableRecord(295, "Joystick7Button14", "Joystick7Button14", 484),
			new KeyCodeTableRecord(296, "Joystick7Button15", "Joystick7Button15", 485),
			new KeyCodeTableRecord(297, "Joystick7Button16", "Joystick7Button16", 486),
			new KeyCodeTableRecord(298, "Joystick7Button17", "Joystick7Button17", 487),
			new KeyCodeTableRecord(299, "Joystick7Button18", "Joystick7Button18", 488),
			new KeyCodeTableRecord(300, "Joystick7Button19", "Joystick7Button19", 489),
			new KeyCodeTableRecord(301, "Joystick8Button0", "Joystick8Button0", 490),
			new KeyCodeTableRecord(302, "Joystick8Button1", "Joystick8Button1", 491),
			new KeyCodeTableRecord(303, "Joystick8Button2", "Joystick8Button2", 492),
			new KeyCodeTableRecord(304, "Joystick8Button3", "Joystick8Button3", 493),
			new KeyCodeTableRecord(305, "Joystick8Button4", "Joystick8Button4", 494),
			new KeyCodeTableRecord(306, "Joystick8Button5", "Joystick8Button5", 495),
			new KeyCodeTableRecord(307, "Joystick8Button6", "Joystick8Button6", 496),
			new KeyCodeTableRecord(308, "Joystick8Button7", "Joystick8Button7", 497),
			new KeyCodeTableRecord(309, "Joystick8Button8", "Joystick8Button8", 498),
			new KeyCodeTableRecord(310, "Joystick8Button9", "Joystick8Button9", 499),
			new KeyCodeTableRecord(311, "Joystick8Button10", "Joystick8Button10", 500),
			new KeyCodeTableRecord(312, "Joystick8Button11", "Joystick8Button11", 501),
			new KeyCodeTableRecord(313, "Joystick8Button12", "Joystick8Button12", 502),
			new KeyCodeTableRecord(314, "Joystick8Button13", "Joystick8Button13", 503),
			new KeyCodeTableRecord(315, "Joystick8Button14", "Joystick8Button14", 504),
			new KeyCodeTableRecord(316, "Joystick8Button15", "Joystick8Button15", 505),
			new KeyCodeTableRecord(317, "Joystick8Button16", "Joystick8Button16", 506),
			new KeyCodeTableRecord(318, "Joystick8Button17", "Joystick8Button17", 507),
			new KeyCodeTableRecord(319, "Joystick8Button18", "Joystick8Button18", 508),
			new KeyCodeTableRecord(320, "Joystick8Button19", "Joystick8Button19", 509)
		};

		public static bool IsActive(this KeyCodeID self)
		{
			if (self >= KeyCodeID.None && self < KeyCodeID.End)
			{
				return self != KeyCodeID.None;
			}
			return false;
		}

		public static bool IsValid(this KeyCodeID self)
		{
			if (self >= KeyCodeID.None)
			{
				return self < KeyCodeID.End;
			}
			return false;
		}

		public static void Clamp(this KeyCodeID self)
		{
			if (self < KeyCodeID.None)
			{
				self = KeyCodeID.None;
			}
			else if ((int)self >= GetEnd())
			{
				self = (KeyCodeID)GetEnd();
			}
		}

		public static int GetEnd(this KeyCodeID self)
		{
			return GetEnd();
		}

		public static KeyCodeID FindID(string enumName)
		{
			for (KeyCodeID keyCodeID = KeyCodeID.None; keyCodeID < KeyCodeID.End; keyCodeID++)
			{
				if (keyCodeID.GetEnumName() == enumName)
				{
					return keyCodeID;
				}
			}
			return KeyCodeID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this KeyCodeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this KeyCodeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this KeyCodeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static int GetValue(this KeyCodeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Value;
			}
			return 0;
		}
	}
}
