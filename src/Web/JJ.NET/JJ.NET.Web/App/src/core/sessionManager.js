let SessionManager = /** @class */ (() => {
    class SessionManager {
        static resetSession() {
            this._isAuthenticated = false;
        }
        static get isAuthenticated() {
            return this._isAuthenticated;
        }
    }
    SessionManager._isAuthenticated = true;
    return SessionManager;
})();
export default SessionManager;
//# sourceMappingURL=sessionManager.js.map