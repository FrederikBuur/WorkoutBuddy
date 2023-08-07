﻿using System.Net;
using WorkoutBuddy.Data.Model;
using WorkoutBuddy.Features.ErrorHandling;
using WorkoutBuddy.Services;

namespace WorkoutBuddy.Controllers;

public class ProfileService : IProfileService
{
    private readonly ILogger<ProfileService> _logger;
    private readonly DataContext _dataContext;
    private readonly UserService _userService;

    public ProfileService(ILogger<ProfileService> logger, DataContext dataContext, UserService userService)
    {
        _logger = logger;
        _dataContext = dataContext;
        _userService = userService;
    }

    public Profile? GetProfile()
    {
        var userId = _userService.Id;
        var profile = _dataContext.Profiles.SingleOrDefault(p => p.UserId == userId);
        return profile;
    }

    public HttpResponseException? ProfileMissingAsException(out Profile? profile)
    {
        profile = GetProfile();
        
        if(profile is null) 
            return new HttpResponseException(HttpStatusCode.Unauthorized, "You dont have an account");
        else 
            return null;
    }
}
