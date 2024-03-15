
namespace Enigmatic.KFInputSystem
{
    //Form Unity KeyCode

    public enum KeyboardKeyCode
    {
        //
        // Сводка:
        //     Not assigned (never returned as the result of a keystroke).
        None = 0,
        //
        // Сводка:
        //     The backspace key.
        Backspace = 8,
        //
        // Сводка:
        //     The forward delete key.
        Delete = 0x7F,
        //
        // Сводка:
        //     The tab key.
        Tab = 9,
        //
        // Сводка:
        //     The Clear key.
        Clear = 12,
        //
        // Сводка:
        //     Return key.
        Return = 13,
        //
        // Сводка:
        //     Pause on PC machines.
        Pause = 19,
        //
        // Сводка:
        //     Escape key.
        Escape = 27,
        //
        // Сводка:
        //     Space key.
        Space = 0x20,
        //
        // Сводка:
        //     Numeric keypad 0.
        Keypad0 = 0x100,
        //
        // Сводка:
        //     Numeric keypad 1.
        Keypad1 = 257,
        //
        // Сводка:
        //     Numeric keypad 2.
        Keypad2 = 258,
        //
        // Сводка:
        //     Numeric keypad 3.
        Keypad3 = 259,
        //
        // Сводка:
        //     Numeric keypad 4.
        Keypad4 = 260,
        //
        // Сводка:
        //     Numeric keypad 5.
        Keypad5 = 261,
        //
        // Сводка:
        //     Numeric keypad 6.
        Keypad6 = 262,
        //
        // Сводка:
        //     Numeric keypad 7.
        Keypad7 = 263,
        //
        // Сводка:
        //     Numeric keypad 8.
        Keypad8 = 264,
        //
        // Сводка:
        //     Numeric keypad 9.
        Keypad9 = 265,
        //
        // Сводка:
        //     Numeric keypad '.'.
        KeypadPeriod = 266,
        //
        // Сводка:
        //     Numeric keypad '/'.
        KeypadDivide = 267,
        //
        // Сводка:
        //     Numeric keypad '*'.
        KeypadMultiply = 268,
        //
        // Сводка:
        //     Numeric keypad '-'.
        KeypadMinus = 269,
        //
        // Сводка:
        //     Numeric keypad '+'.
        KeypadPlus = 270,
        //
        // Сводка:
        //     Numeric keypad Enter.
        KeypadEnter = 271,
        //
        // Сводка:
        //     Numeric keypad '='.
        KeypadEquals = 272,
        //
        // Сводка:
        //     Up arrow key.
        UpArrow = 273,
        //
        // Сводка:
        //     Down arrow key.
        DownArrow = 274,
        //
        // Сводка:
        //     Right arrow key.
        RightArrow = 275,
        //
        // Сводка:
        //     Left arrow key.
        LeftArrow = 276,
        //
        // Сводка:
        //     Insert key key.
        Insert = 277,
        //
        // Сводка:
        //     Home key.
        Home = 278,
        //
        // Сводка:
        //     End key.
        End = 279,
        //
        // Сводка:
        //     Page up.
        PageUp = 280,
        //
        // Сводка:
        //     Page down.
        PageDown = 281,
        //
        // Сводка:
        //     F1 function key.
        F1 = 282,
        //
        // Сводка:
        //     F2 function key.
        F2 = 283,
        //
        // Сводка:
        //     F3 function key.
        F3 = 284,
        //
        // Сводка:
        //     F4 function key.
        F4 = 285,
        //
        // Сводка:
        //     F5 function key.
        F5 = 286,
        //
        // Сводка:
        //     F6 function key.
        F6 = 287,
        //
        // Сводка:
        //     F7 function key.
        F7 = 288,
        //
        // Сводка:
        //     F8 function key.
        F8 = 289,
        //
        // Сводка:
        //     F9 function key.
        F9 = 290,
        //
        // Сводка:
        //     F10 function key.
        F10 = 291,
        //
        // Сводка:
        //     F11 function key.
        F11 = 292,
        //
        // Сводка:
        //     F12 function key.
        F12 = 293,
        //
        // Сводка:
        //     F13 function key.
        F13 = 294,
        //
        // Сводка:
        //     F14 function key.
        F14 = 295,
        //
        // Сводка:
        //     F15 function key.
        F15 = 296,
        //
        // Сводка:
        //     The '0' key on the top of the alphanumeric keyboard.
        Alpha0 = 48,
        //
        // Сводка:
        //     The '1' key on the top of the alphanumeric keyboard.
        Alpha1 = 49,
        //
        // Сводка:
        //     The '2' key on the top of the alphanumeric keyboard.
        Alpha2 = 50,
        //
        // Сводка:
        //     The '3' key on the top of the alphanumeric keyboard.
        Alpha3 = 51,
        //
        // Сводка:
        //     The '4' key on the top of the alphanumeric keyboard.
        Alpha4 = 52,
        //
        // Сводка:
        //     The '5' key on the top of the alphanumeric keyboard.
        Alpha5 = 53,
        //
        // Сводка:
        //     The '6' key on the top of the alphanumeric keyboard.
        Alpha6 = 54,
        //
        // Сводка:
        //     The '7' key on the top of the alphanumeric keyboard.
        Alpha7 = 55,
        //
        // Сводка:
        //     The '8' key on the top of the alphanumeric keyboard.
        Alpha8 = 56,
        //
        // Сводка:
        //     The '9' key on the top of the alphanumeric keyboard.
        Alpha9 = 57,
        //
        // Сводка:
        //     Exclamation mark key '!'. Deprecated if "Use Physical Keys" is enabled in instead.
        Exclaim = 33,
        //
        // Сводка:
        //     Double quote key '"'. Deprecated if "Use Physical Keys" is enabled in instead.
        DoubleQuote = 34,
        //
        // Сводка:
        //     Hash key '#'. Deprecated if "Use Physical Keys" is enabled in instead.
        Hash = 35,
        //
        // Сводка:
        //     Dollar sign key '$'. Deprecated if "Use Physical Keys" is enabled in instead.
        Dollar = 36,
        //
        // Сводка:
        //     Percent '%' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Percent = 37,
        //
        // Сводка:
        //     Ampersand key '&'. Deprecated if "Use Physical Keys" is enabled in instead.
        Ampersand = 38,
        //
        // Сводка:
        //     Quote key '.
        Quote = 39,
        //
        // Сводка:
        //     Left Parenthesis key '('. Deprecated if "Use Physical Keys" is enabled in instead.
        LeftParen = 40,
        //
        // Сводка:
        //     Right Parenthesis key ')'. Deprecated if "Use Physical Keys" is enabled in instead.
        RightParen = 41,
        //
        // Сводка:
        //     Asterisk key '*'. Deprecated if "Use Physical Keys" is enabled in instead.
        Asterisk = 42,
        //
        // Сводка:
        //     Plus key '+'. Deprecated if "Use Physical Keys" is enabled in instead.
        Plus = 43,
        //
        // Сводка:
        //     Comma ',' key.
        Comma = 44,
        //
        // Сводка:
        //     Minus '-' key.
        Minus = 45,
        //
        // Сводка:
        //     Period '.' key.
        Period = 46,
        //
        // Сводка:
        //     Slash '/' key.
        Slash = 47,
        //
        // Сводка:
        //     Colon ':' key.Deprecated if "Use Physical Keys" is enabled in instead.
        Colon = 58,
        //
        // Сводка:
        //     Semicolon ';' key.
        Semicolon = 59,
        //
        // Сводка:
        //     Less than '<' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Less = 60,
        //
        // Сводка:
        //     Equals '=' key.
        Equals = 61,
        //
        // Сводка:
        //     Greater than '>' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Greater = 62,
        //
        // Сводка:
        //     Question mark '?' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Question = 0x3F,
        //
        // Сводка:
        //     At key '@'. Deprecated if "Use Physical Keys" is enabled in instead.
        At = 0x40,
        //
        // Сводка:
        //     Left square bracket key '['.
        LeftBracket = 91,
        //
        // Сводка:
        //     Backslash key '\'.
        Backslash = 92,
        //
        // Сводка:
        //     Right square bracket key ']'.
        RightBracket = 93,
        //
        // Сводка:
        //     Caret key '^'. Deprecated if "Use Physical Keys" is enabled in instead.
        Caret = 94,
        //
        // Сводка:
        //     Underscore '_' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Underscore = 95,
        //
        // Сводка:
        //     Back quote key '`'.
        BackQuote = 96,
        //
        // Сводка:
        //     'a' key.
        A = 97,
        //
        // Сводка:
        //     'b' key.
        B = 98,
        //
        // Сводка:
        //     'c' key.
        C = 99,
        //
        // Сводка:
        //     'd' key.
        D = 100,
        //
        // Сводка:
        //     'e' key.
        E = 101,
        //
        // Сводка:
        //     'f' key.
        F = 102,
        //
        // Сводка:
        //     'g' key.
        G = 103,
        //
        // Сводка:
        //     'h' key.
        H = 104,
        //
        // Сводка:
        //     'i' key.
        I = 105,
        //
        // Сводка:
        //     'j' key.
        J = 106,
        //
        // Сводка:
        //     'k' key.
        K = 107,
        //
        // Сводка:
        //     'l' key.
        L = 108,
        //
        // Сводка:
        //     'm' key.
        M = 109,
        //
        // Сводка:
        //     'n' key.
        N = 110,
        //
        // Сводка:
        //     'o' key.
        O = 111,
        //
        // Сводка:
        //     'p' key.
        P = 112,
        //
        // Сводка:
        //     'q' key.
        Q = 113,
        //
        // Сводка:
        //     'r' key.
        R = 114,
        //
        // Сводка:
        //     's' key.
        S = 115,
        //
        // Сводка:
        //     't' key.
        T = 116,
        //
        // Сводка:
        //     'u' key.
        U = 117,
        //
        // Сводка:
        //     'v' key.
        V = 118,
        //
        // Сводка:
        //     'w' key.
        W = 119,
        //
        // Сводка:
        //     'x' key.
        X = 120,
        //
        // Сводка:
        //     'y' key.
        Y = 121,
        //
        // Сводка:
        //     'z' key.
        Z = 122,
        //
        // Сводка:
        //     Left curly bracket key '{'. Deprecated if "Use Physical Keys" is enabled in instead.
        LeftCurlyBracket = 123,
        //
        // Сводка:
        //     Pipe '|' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Pipe = 124,
        //
        // Сводка:
        //     Right curly bracket key '}'. Deprecated if "Use Physical Keys" is enabled in
        //     instead.
        RightCurlyBracket = 125,
        //
        // Сводка:
        //     Tilde '~' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Tilde = 126,
        //
        // Сводка:
        //     Numlock key.
        Numlock = 300,
        //
        // Сводка:
        //     Capslock key.
        CapsLock = 301,
        //
        // Сводка:
        //     Scroll lock key.
        ScrollLock = 302,
        //
        // Сводка:
        //     Right shift key.
        RightShift = 303,
        //
        // Сводка:
        //     Left shift key.
        LeftShift = 304,
        //
        // Сводка:
        //     Right Control key.
        RightControl = 305,
        //
        // Сводка:
        //     Left Control key.
        LeftControl = 306,
        //
        // Сводка:
        //     Right Alt key.
        RightAlt = 307,
        //
        // Сводка:
        //     Left Alt key.
        LeftAlt = 308,
        //
        // Сводка:
        //     Maps to left Windows key or left Command key if physical keys are enabled in
        //     Input Manager settings, otherwise maps to left Command key only.
        LeftMeta = 310,
        //
        // Сводка:
        //     Left Command key.
        LeftCommand = 310,
        //
        // Сводка:
        //     Left Command key.
        LeftApple = 310,
        //
        // Сводка:
        //     Left Windows key. Deprecated if "Use Physical Keys" is enabled in instead.
        LeftWindows = 311,
        //
        // Сводка:
        //     Maps to right Windows key or right Command key if physical keys are enabled in
        //     Input Manager settings, otherwise maps to right Command key only.
        RightMeta = 309,
        //
        // Сводка:
        //     Right Command key.
        RightCommand = 309,
        //
        // Сводка:
        //     Right Command key.
        RightApple = 309,
        //
        // Сводка:
        //     Right Windows key. Deprecated if "Use Physical Keys" is enabled in instead.
        RightWindows = 312,
        //
        // Сводка:
        //     Alt Gr key. Deprecated if "Use Physical Keys" is enabled in instead.
        AltGr = 313,
        //
        // Сводка:
        //     Help key. Deprecated if "Use Physical Keys" is enabled in, doesn't map to any
        //     physical key.
        Help = 315,
        //
        // Сводка:
        //     Print key.
        Print = 316,
        //
        // Сводка:
        //     Sys Req key. Deprecated if "Use Physical Keys" is enabled in, doesn't map to
        //     any physical key.
        SysReq = 317,
        //
        // Сводка:
        //     Break key. Deprecated if "Use Physical Keys" is enabled in, doesn't map to any
        //     physical key.
        Break = 318,
        //
        // Сводка:
        //     Menu key.
        Menu = 319,
    }

    public enum MouseButtonCode
    {
        //
        // Сводка:
        //     The Left (or primary) mouse button.
        Mouse0 = 323,
        //
        // Сводка:
        //     Right mouse button (or secondary mouse button).
        Mouse1 = 324,
        //
        // Сводка:
        //     Middle mouse button (or third button).
        Mouse2 = 325,
        //
        // Сводка:
        //     Additional (fourth) mouse button.
        Mouse3 = 326,
        //
        // Сводка:
        //     Additional (fifth) mouse button.
        Mouse4 = 327,
        //
        // Сводка:
        //     Additional (or sixth) mouse button.
        Mouse5 = 328,
        //
        // Сводка:
        //     Additional (or seventh) mouse button.
        Mouse6 = 329
    }

    public enum JoystickButtonCode
    {
        //
        // Сводка:
        //     Button 0 on any joystick.
        JoystickButton0 = 330,
        //
        // Сводка:
        //     Button 1 on any joystick.
        JoystickButton1 = 331,
        //
        // Сводка:
        //     Button 2 on any joystick.
        JoystickButton2 = 332,
        //
        // Сводка:
        //     Button 3 on any joystick.
        JoystickButton3 = 333,
        //
        // Сводка:
        //     Button 4 on any joystick.
        JoystickButton4 = 334,
        //
        // Сводка:
        //     Button 5 on any joystick.
        JoystickButton5 = 335,
        //
        // Сводка:
        //     Button 6 on any joystick.
        JoystickButton6 = 336,
        //
        // Сводка:
        //     Button 7 on any joystick.
        JoystickButton7 = 337,
        //
        // Сводка:
        //     Button 8 on any joystick.
        JoystickButton8 = 338,
        //
        // Сводка:
        //     Button 9 on any joystick.
        JoystickButton9 = 339,
        //
        // Сводка:
        //     Button 10 on any joystick.
        JoystickButton10 = 340,
        //
        // Сводка:
        //     Button 11 on any joystick.
        JoystickButton11 = 341,
        //
        // Сводка:
        //     Button 12 on any joystick.
        JoystickButton12 = 342,
        //
        // Сводка:
        //     Button 13 on any joystick.
        JoystickButton13 = 343,
        //
        // Сводка:
        //     Button 14 on any joystick.
        JoystickButton14 = 344,
        //
        // Сводка:
        //     Button 15 on any joystick.
        JoystickButton15 = 345,
        //
        // Сводка:
        //     Button 16 on any joystick.
        JoystickButton16 = 346,
        //
        // Сводка:
        //     Button 17 on any joystick.
        JoystickButton17 = 347,
        //
        // Сводка:
        //     Button 18 on any joystick.
        JoystickButton18 = 348,
        //
        // Сводка:
        //     Button 19 on any joystick.
        JoystickButton19 = 349,
    }

    public enum JoystickAxis
    {
        None = 0,
        LeftStickX,
        LeftStickY,
        RightStickX,
        RightStickY
    }

    public enum MouseAxis
    {
        None = 0,
        MouseX,
        MouseY
    }
}