﻿using System;

namespace Nbtc.Network
{
    public class Result<T>
    {
        private Result()
        {
        }

        public T Data { get; private set; }
        public bool Valid { get; private set; }
        public ErrorEnum Error { get; private set; }
        public Exception Exception { get; private set; }

        public static Result<T> Fail(ErrorEnum error, Exception e = null)
        {
            return new Result<T> {Error = error, Exception = e};
        }

        public static Result<T> Succeed(T data)
        {
            return new Result<T> {Valid = true, Data = data};
        }
    }
}