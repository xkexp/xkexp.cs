﻿namespace xkexp.OfOrleans.Basics.GrainInterfaces;

public interface IHello : Orleans.IGrainWithIntegerKey
{
    Task<string> SayHello(string greeting);
}
