using System;

public static class EnumConverter
{
    // enum을 string으로 return
    public static string GetString<T>(T e) where T : Enum
    {
        return e.ToString();
    }

    // enum을 int로 return
    public static int GetInt<T>(T e) where T : Enum
    {
        return Convert.ToInt32(e);
    }
}
