import moment from 'moment';
import 'moment/locale/ta';

export const formatDateRelative = (date) => {
    //moment.locale('ta');
    return moment(date).fromNow();
};