import gql from 'graphql-tag'

export const MESSAGE_QUERY_ID= gql`
query Message ($id:String!,$userId:String!,$activePage:Int!,$pageSize:Int!){
    message(id: $id, userId:$userId, activePage: $activePage, pageSize: $pageSize) {
        totalCount
        totalPageCount
        startUserId
        toUserId
        id
        messages {
            id
            blockUserId
            isBloked
            createDate
            isRead
            userId
            message
        }
    }
}
`;