using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang
{
    enum ASCII
    {
        NUL=0,
        SOH=1,
        STX=2,
        ETX=3,
        EOT=4,
        ENQ=5,
        ACK=6,
        BEL=7,
        BS=8,
        HT=9,
        LF=10,
        VT=11,
        FF=12,
        CR=13,
        SO=14,
        SI=15,
        DLE=16,
        DC1=17,
        DC2=18,
        DC3=19,
        DC4=20,
        NAK=21,
        SYN=22,
        ETB=23,
        CAN=24,
        EM=25,
        SUB=26,
        ESC=27,
        FS=28,
        GS=29,
        RS=30,
        US=31,
        //
        // Summary:
        //     Space key.
        Space = 32,
        //
        // Summary:
        //     Exclamation mark key '!'.
        Exclaim = 33,
        //
        // Summary:
        //     Double quote key '"'.
        DoubleQuote = 34,
        //
        // Summary:
        //     Hash key '#'.
        Hash = 35,
        //
        // Summary:
        //     Dollar sign key '$'.
        Dollar = 36,
        //
        // Summary:
        //     Percent '%' key.
        Percent = 37,
        //
        // Summary:
        //     Ampersand key '&'.
        Ampersand = 38,
        //
        // Summary:
        //     Quote key '.
        Quote = 39,
        //
        // Summary:
        //     Left Parenthesis key '('.
        LeftParen = 40,
        //
        // Summary:
        //     Right Parenthesis key ')'.
        RightParen = 41,
        //
        // Summary:
        //     Asterisk key '*'.
        Asterisk = 42,
        //
        // Summary:
        //     Plus key '+'.
        Plus = 43,
        //
        // Summary:
        //     Comma ',' key.
        Comma = 44,
        //
        // Summary:
        //     Minus '-' key.
        Minus = 45,
        //
        // Summary:
        //     Period '.' key.
        Period = 46,
        //
        // Summary:
        //     Slash '/' key.
        Slash = 47,
        //
        // Summary:
        //     The '0' key on the top of the alphanumeric keyboard.
        Alpha0 = 48,
        //
        // Summary:
        //     The '1' key on the top of the alphanumeric keyboard.
        Alpha1 = 49,
        //
        // Summary:
        //     The '2' key on the top of the alphanumeric keyboard.
        Alpha2 = 50,
        //
        // Summary:
        //     The '3' key on the top of the alphanumeric keyboard.
        Alpha3 = 51,
        //
        // Summary:
        //     The '4' key on the top of the alphanumeric keyboard.
        Alpha4 = 52,
        //
        // Summary:
        //     The '5' key on the top of the alphanumeric keyboard.
        Alpha5 = 53,
        //
        // Summary:
        //     The '6' key on the top of the alphanumeric keyboard.
        Alpha6 = 54,
        //
        // Summary:
        //     The '7' key on the top of the alphanumeric keyboard.
        Alpha7 = 55,
        //
        // Summary:
        //     The '8' key on the top of the alphanumeric keyboard.
        Alpha8 = 56,
        //
        // Summary:
        //     The '9' key on the top of the alphanumeric keyboard.
        Alpha9 = 57,
        //
        // Summary:
        //     Colon ':' key.
        Colon = 58,
        //
        // Summary:
        //     Semicolon ';' key.
        Semicolon = 59,
        //
        // Summary:
        //     Less than '<' key.
        Less = 60,
        //
        // Summary:
        //     Equals '=' key.
        Equals = 61,
        //
        // Summary:
        //     Greater than '>' key.
        Greater = 62,
        //
        // Summary:
        //     Question mark '?' key.
        Question = 63,
        //
        // Summary:
        //     At key '@'.
        At = 64,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        //
        // Summary:
        //     Left square bracket key '['.
        LeftBracket = 91,
        //
        // Summary:
        //     Backslash key '\'.
        Backslash = 92,
        //
        // Summary:
        //     Right square bracket key ']'.
        RightBracket = 93,
        //
        // Summary:
        //     Caret key '^'.
        Caret = 94,
        //
        // Summary:
        //     Underscore '_' key.
        Underscore = 95,
        //
        // Summary:
        //     Back quote key '`'.
        BackQuote = 96,
        a = 97,
        b = 98,
        c = 99,
        d = 100,
        e = 101,
        f = 102,
        g = 103,
        h = 104,
        i = 105,
        j = 106,
        k = 107,
        l = 108,
        m = 109,
        n = 110,
        o = 111,
        p = 112,
        q = 113,
        r = 114,
        s = 115,
        t = 116,
        u = 117,
        v = 118,
        w = 119,
        x = 120,
        y = 121,
        z = 122,
        //
        // Summary:
        //     Left curly bracket key '{'.
        LeftCurlyBracket = 123,
        //
        // Summary:
        //     Pipe '|' key.
        Pipe = 124,
        //
        // Summary:
        //     Right curly bracket key '}'.
        RightCurlyBracket = 125,
        //
        // Summary:
        //     Tilde '~' key.
        Tilde = 126,
        //
        // Summary:
        //     The forward delete key.
        Delete = 127
    }
}
