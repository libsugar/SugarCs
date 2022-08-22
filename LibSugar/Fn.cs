using System;

namespace LibSugar;

public static partial class Sugar
{
    /// <summary>
    /// Identity function
    /// </summary>
    public static T Identity<T>(T v) => v;

    /// <summary>
    /// Do nothing
    /// </summary>
    public static void Empty() { }

    /// <summary>
    /// throw TODO
    /// </summary>
    /// <param name="message">TODO message</param>
    /// <exception cref="NotImplementedException"></exception>
    public static void TODO(string? message = "TODO")
    {
        throw new NotImplementedException(message);
    }
    /// <summary>
    /// throw TODO
    /// </summary>
    /// <param name="message">TODO message</param>
    /// <param name="inner">Inner Exception</param>
    /// <exception cref="NotImplementedException"></exception>
    public static void TODO(string? message, Exception? inner)
    {
        throw new NotImplementedException(message, inner);
    }
    /// <summary>
    /// throw TODO
    /// </summary>
    /// <param name="inner">Inner Exception</param>
    /// <exception cref="NotImplementedException"></exception>
    public static void TODO(Exception? inner)
    {
        throw new NotImplementedException("TODO", inner);
    }
    /// <summary>
    /// throw TODO
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="message">TODO message</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static T TODO<T>(string? message = "TODO")
    {
        throw new NotImplementedException(message);
    }
    /// <summary>
    /// throw TODO
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="message">TODO message</param>
    /// <param name="inner">Inner Exception</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static T TODO<T>(string? message, Exception? inner)
    {
        throw new NotImplementedException(message, inner);
    }
    /// <summary>
    /// throw TODO
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="inner">Inner Exception</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static T TODO<T>(Exception? inner)
    {
        throw new NotImplementedException("TODO", inner);
    }
}
