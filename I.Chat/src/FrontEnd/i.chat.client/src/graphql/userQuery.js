import gql from 'graphql-tag'

export const USER_MUTATION_SIGNUP = gql`
mutation Register ($email:String!,$userName:String!,$password:String!){
    register(input: { dtoModel: { email: $email, userName: $userName,password: $password} }) {
        stateResult {
            fields
            message
            stateStatus
            isError
        }
    }
}
`;

export const USER_MUTATION_LOGIN = gql`
mutation Login ($email:String!,$password:String!){
    login(input: { dtoModel: {email: $email,password: $password} }) {
        stateResult {
            fields
            message
            stateStatus
            isError
        }
    }
}
`;

export const USER_QUERY_ID = gql`
query User($id:String!) {
    user(id: $id) {
        id
        userName
        email
    }
}
`;

export const USER_QUERY_SEARCH = gql`
query SearchUsers ($userName:String!,$userId:String!){
    searchUsers(username: $userName,userId: $userId) {
        id
        userId
        name
        avatar
    }
}
`;

export const USER_MUTATION_UPDATE_PASSWORD = gql`
mutation UpdatePassword ($id: String!, $oldPassword: String!, $newPassword: String!){
    updatePassword(input: { dtoModel: { id: $id,newPassword: $newPassword,oldPassword: $oldPassword } }) {
        stateResult {
            fields
            message
            stateStatus
            isError
        }
    }
}
`;

export const USER_MUTATION_UPDATE_USERNAME = gql`
 mutation UpdateUserName ($id: String!, $userName: String!){
    updateUserName(input: { dtoModel: { id: $id,userName: $userName} }) {
        stateResult {
            fields
            message
            stateStatus
            isError
        }
    }
}
`;
