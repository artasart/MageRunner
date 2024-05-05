using System;
using System.Text;
using System.Threading.Tasks;
using AppleAuth.Interfaces;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseAppleLogin : MonoBehaviour
{
	private FirebaseAuth firebaseAuth;

	public void PerformFirebaseAuthentication(
		IAppleIDCredential appleIdCredential,
		string rawNonce,
		Action<FirebaseUser> firebaseAuthCallback)
	{
		var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
		var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
		var firebaseCredential = OAuthProvider.GetCredential(
			"apple.com",
			identityToken,
			rawNonce,
			authorizationCode);

		this.firebaseAuth.SignInWithCredentialAsync(firebaseCredential)
			.ContinueWithOnMainThread(task => HandleSignInWithUser(task, firebaseAuthCallback));
	}

	private static void HandleSignInWithUser(Task<FirebaseUser> task, Action<FirebaseUser> firebaseUserCallback)
	{
		if (task.IsCanceled)
		{
			Debug.Log("Firebase auth was canceled");
			firebaseUserCallback(null);
		}
		else if (task.IsFaulted)
		{
			Debug.Log("Firebase auth failed");
			firebaseUserCallback(null);
		}
		else
		{
			var firebaseUser = task.Result;
			Debug.Log("Firebase auth completed | User ID:" + firebaseUser.UserId);
			firebaseUserCallback(firebaseUser);
		}
	}
}