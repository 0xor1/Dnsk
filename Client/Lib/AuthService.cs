﻿using Dnsk.Common;
using Dnsk.Proto;

namespace Dnsk.Client.Lib;

public record Session(string Id, bool IsAuthed);

public interface IAuthService
{
    Task<Session> GetSession();
    Task Register(string email, string pwd);
    Task VerifyEmail(string email, string code);
    Task<Session> Login(string email, string pwd);
    Task<Session> Logout();
}

public class AuthService: IAuthService
{
    private Session? _session;
    private readonly Api.ApiClient _api;
    
    public AuthService(Api.ApiClient api)
    {
        _api = api;
    }

    public async Task<Session> GetSession()
    {
        if (_session == null)
        {
            var ses = await _api.Auth_GetSessionAsync(new Nothing());
            _session = new Session(ses.Id, ses.IsAuthed);
        }
        return _session;
    }

    public async Task Register(string email, string pwd)
    {
        var ses = await GetSession();
        Throw.OpIf(ses.IsAuthed, "already in authenticated session");
        await _api.Auth_RegisterAsync(new Auth_RegisterReq()
        {
            Email = email,
            Pwd = pwd
        });
    }

    public async Task VerifyEmail(string email, string code)
    {
        var ses = await GetSession();
        Throw.OpIf(ses.IsAuthed, "already in authenticated session");
        await _api.Auth_VerifyEmailAsync(new Auth_VerifyEmailReq()
        {
            Email = email,
            Code = code
        });
    }

    public async Task<Session> Login(string email, string pwd)
    {
        var ses = await GetSession();
        Throw.OpIf(ses.IsAuthed, "already in authenticated session");
        var newSes = await _api.Auth_LoginAsync(new Auth_LoginReq()
        {
            Email = email,
            Pwd = pwd
        });
        _session = new Session(newSes.Id, newSes.IsAuthed);
        return _session;
    }

    public async Task<Session> Logout()
    {
        var ses = await GetSession();
        if (!ses.IsAuthed)
        {
            return ses;
        }
        var newSes = await _api.Auth_LogoutAsync(new Nothing());
        _session = new Session(newSes.Id, newSes.IsAuthed);
        return _session;
    }
}