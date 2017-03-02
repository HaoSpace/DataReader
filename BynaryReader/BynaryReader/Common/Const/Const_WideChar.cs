using System;

[StructureAttribute(16 * 2)]
public struct WideChar16
{
    [FieldAttribute(1, 16 * 2)]
    public byte[] Text;

    public static implicit operator string (WideChar16 w)
    {
        if (w.Text == null)
            return string.Empty;
        else
            return System.Text.Encoding.Unicode.GetString(w.Text);
    }

    public static implicit operator WideChar16 (string s)
    {
        WideChar16 w = new WideChar16();
        w.Text = System.Text.Encoding.Unicode.GetBytes(s);
        return w;
    }

    public static bool operator == (WideChar16 w1, WideChar16 w2)
    {
        string s1 = System.Text.Encoding.Unicode.GetString(w1.Text);
        string s2 = System.Text.Encoding.Unicode.GetString(w2.Text);
        return string.Equals(s1, s2);
    }

    public static bool operator != (WideChar16 w1, WideChar16 w2)
    {
        string s1 = System.Text.Encoding.Unicode.GetString(w1.Text);
        string s2 = System.Text.Encoding.Unicode.GetString(w2.Text);
        return !string.Equals(s1, s2);
    }

    public override bool Equals (object obj)
    {
        if (!(obj is WideChar16))
            return false;

        WideChar16 w = (WideChar16)obj;

        return this == w;
    }

    public override int GetHashCode ()
    {
        return base.GetHashCode();
    }

    public override string ToString ()
    {
        if (Text == null)
            return string.Empty;
        else
            return System.Text.Encoding.Unicode.GetString(Text);
    }
}

[StructureAttribute(32 * 2)]
public struct WideChar32
{
    [FieldAttribute(1, 32 * 2)]
    public byte[] Text;

    public static implicit operator string (WideChar32 w)
    {
        if (w.Text == null)
            return string.Empty;
        else
            return System.Text.Encoding.Unicode.GetString(w.Text);
    }

    public static implicit operator WideChar32 (string s)
    {
        WideChar32 w = new WideChar32();
        w.Text = System.Text.Encoding.Unicode.GetBytes(s);
        return w;
    }

    public static bool operator == (WideChar32 w1, WideChar32 w2)
    {
        string s1 = System.Text.Encoding.Unicode.GetString(w1.Text);
        string s2 = System.Text.Encoding.Unicode.GetString(w2.Text);
        return string.Equals(s1, s2);
    }

    public static bool operator != (WideChar32 w1, WideChar32 w2)
    {
        string s1 = System.Text.Encoding.Unicode.GetString(w1.Text);
        string s2 = System.Text.Encoding.Unicode.GetString(w2.Text);
        return !string.Equals(s1, s2);
    }

    public override bool Equals (object obj)
    {
        if (!(obj is WideChar32))
            return false;

        WideChar32 w = (WideChar32)obj;

        return this == w;
    }

    public override int GetHashCode ()
    {
        return base.GetHashCode();
    }

    public override string ToString ()
    {
        if (Text == null)
            return string.Empty;
        else
            return System.Text.Encoding.Unicode.GetString(Text);
    }
}

[StructureAttribute(64 * 2)]
public struct WideChar64
{
    [FieldAttribute(1, 64 * 2)]
    public byte[] Text;

    public static implicit operator string (WideChar64 w)
    {
        if (w.Text == null)
            return string.Empty;
        else
            return System.Text.Encoding.Unicode.GetString(w.Text);
    }

    public static implicit operator WideChar64 (string s)
    {
        WideChar64 w = new WideChar64();
        w.Text = System.Text.Encoding.Unicode.GetBytes(s);
        return w;
    }

    public static bool operator == (WideChar64 w1, WideChar64 w2)
    {
        string s1 = System.Text.Encoding.Unicode.GetString(w1.Text);
        string s2 = System.Text.Encoding.Unicode.GetString(w2.Text);
        return string.Equals(s1, s2);
    }

    public static bool operator != (WideChar64 w1, WideChar64 w2)
    {
        string s1 = System.Text.Encoding.Unicode.GetString(w1.Text);
        string s2 = System.Text.Encoding.Unicode.GetString(w2.Text);
        return !string.Equals(s1, s2);
    }

    public override bool Equals (object obj)
    {
        if (!(obj is WideChar64))
            return false;

        WideChar64 w = (WideChar64)obj;

        return this == w;
    }

    public override int GetHashCode ()
    {
        return base.GetHashCode();
    }

    public override string ToString ()
    {
        if (Text == null)
            return string.Empty;
        else
            return System.Text.Encoding.Unicode.GetString(Text);
    }
}

[StructureAttribute(100 * 2)]
public struct WideChar100
{
    [FieldAttribute(1, 100 * 2)]
    public byte[] Text;

    public static implicit operator string (WideChar100 w)
    {
        if (w.Text == null)
            return string.Empty;
        else
            return System.Text.Encoding.Unicode.GetString(w.Text);
    }

    public static implicit operator WideChar100 (string s)
    {
        WideChar100 w = new WideChar100();
        w.Text = System.Text.Encoding.Unicode.GetBytes(s);
        return w;
    }

    public static bool operator == (WideChar100 w1, WideChar100 w2)
    {
        string s1 = System.Text.Encoding.Unicode.GetString(w1.Text);
        string s2 = System.Text.Encoding.Unicode.GetString(w2.Text);
        return string.Equals(s1, s2);
    }

    public static bool operator != (WideChar100 w1, WideChar100 w2)
    {
        string s1 = System.Text.Encoding.Unicode.GetString(w1.Text);
        string s2 = System.Text.Encoding.Unicode.GetString(w2.Text);
        return !string.Equals(s1, s2);
    }

    public override bool Equals (object obj)
    {
        if (!(obj is WideChar100))
            return false;

        WideChar100 w = (WideChar100)obj;

        return this == w;
    }

    public override int GetHashCode ()
    {
        return base.GetHashCode();
    }

    public override string ToString ()
    {
        if (Text == null)
            return string.Empty;
        else
            return System.Text.Encoding.Unicode.GetString(Text);
    }
}

[StructureAttribute(200 * 2)]
public struct WideChar200
{
    [FieldAttribute(1, 200 * 2)]
    public byte[] Text;

    public static implicit operator string (WideChar200 w)
    {
        if (w.Text == null)
            return string.Empty;
        else
            return System.Text.Encoding.Unicode.GetString(w.Text);
    }

    public static implicit operator WideChar200 (string s)
    {
        WideChar200 w = new WideChar200();
        w.Text = System.Text.Encoding.Unicode.GetBytes(s);
        return w;
    }

    public static bool operator == (WideChar200 w1, WideChar200 w2)
    {
        string s1 = System.Text.Encoding.Unicode.GetString(w1.Text);
        string s2 = System.Text.Encoding.Unicode.GetString(w2.Text);
        return string.Equals(s1, s2);
    }

    public static bool operator != (WideChar200 w1, WideChar200 w2)
    {
        string s1 = System.Text.Encoding.Unicode.GetString(w1.Text);
        string s2 = System.Text.Encoding.Unicode.GetString(w2.Text);
        return !string.Equals(s1, s2);
    }

    public override bool Equals (object obj)
    {
        if (!(obj is WideChar200))
            return false;

        WideChar200 w = (WideChar200)obj;

        return this == w;
    }

    public override int GetHashCode ()
    {
        return base.GetHashCode();
    }

    public override string ToString ()
    {
        if (Text == null)
            return string.Empty;
        else
            return System.Text.Encoding.Unicode.GetString(Text);
    }
}