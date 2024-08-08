import http from 'k6/http';
import { sleep } from 'k6';

export default function () {
  http.get('http://localhost:5678/api/testA/forward');
  sleep(5);
}