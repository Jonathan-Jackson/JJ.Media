export default class SessionManager {
  private static _isAuthenticated = true;

  public static resetSession(): void {
    this._isAuthenticated = false;
  }

  public static get isAuthenticated(): boolean {
    return this._isAuthenticated;
  }
}
