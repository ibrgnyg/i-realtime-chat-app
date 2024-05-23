import gql from 'graphql-tag'

export const USER_CONNECTIONID_MUTATION_UPDATE= gql`
mutation UpdateUserConnectionId ($userId:String!,$connectionId:String!){
    updateUserConnectionId(input: { model: { userId: $userId, connectionId: $connectionId } }) {
        stateResult {
            fields
            message
            stateStatus
            isError
        }
    }
}
`;