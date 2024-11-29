//used to protect url from not authenticated user
// https://authjs.dev/getting-started/session-management/protecting
export { auth as middleware } from "@/auth"

export const config = {
    matcher: [
        '/session',
    ],
    pages:{
        signIn: '/api/auth/signin'
    }
  }