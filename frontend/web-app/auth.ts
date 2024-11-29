import NextAuth, { Profile } from "next-auth"
import { OIDCConfig } from "next-auth/providers"
import DuendeIDS6Provider from "next-auth/providers/duende-identity-server6"
 
export const { handlers, signIn, signOut, auth } = NextAuth({
    debug:true,
    session:{
        strategy:'jwt'
    },
  providers: [
    DuendeIDS6Provider({
            id: 'id-server',
            clientId: "nextApp",
            clientSecret: "secret",
            issuer: "http://localhost:5000",
            authorization:{params:{
                    scope:'openid profile auctionApp'
                }},
            idToken: true
        } as OIDCConfig<Omit<Profile, 'username'>>),
    ],
    
    callbacks: {
         authorized: async ({ auth }) => {
            // Logged in users are authenticated, otherwise redirect to login page
            return !!auth;
        },

        //extending session
        //https://authjs.dev/guides/extending-the-session
        async jwt({token, profile, account}){
            if(account && account.access_token) 
                token.accessToken = account.access_token;
            if(profile)
                token.username = profile.username
            return token;
        },
        async session({session, token}){
           if(token){
            session.user.username = token.username;
            session.accessToken = token.accessToken;

           }
            
            return session;
        }
    }
})