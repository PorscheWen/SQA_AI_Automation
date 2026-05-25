#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""啟動購物車示範頁的本地 HTTP 伺服器（供 TestComplete 測試使用）。"""

import http.server
import os
import socketserver

PORT = 8888
DIRECTORY = os.path.dirname(os.path.abspath(__file__))


class Handler(http.server.SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=DIRECTORY, **kwargs)


class ThreadingHTTPServer(socketserver.ThreadingMixIn, socketserver.TCPServer):
    allow_reuse_address = True
    daemon_threads = True


if __name__ == "__main__":
    with ThreadingHTTPServer(("", PORT), Handler) as httpd:
        print("Shopping cart demo running at http://localhost:%d/" % PORT)
        print("Press Ctrl+C to stop.")
        httpd.serve_forever()
